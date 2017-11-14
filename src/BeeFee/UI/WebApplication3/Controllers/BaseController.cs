using System;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.Model;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Models.Interfaces;

namespace WebApplication3.Controllers
{
	public abstract class BaseController<TService> : Controller where TService : BaseBeefeeService
	{
		protected readonly TService Service;

		protected BaseController(TService service)
		{
			Service = service.HasNotNullArg("Service");
		}

		protected bool IsAjax => Request.Headers.ContainsKey("X-Requested-With") &&
								Request.Headers["X-Requested-With"] == "XMLHttpRequest";

		protected bool IsJson => Request.Headers.ContainsKey("AcceptTypes") &&
								Request.Headers["AcceptTypes"].Any(x => x.ToLower().IndexOf("json", StringComparison.Ordinal) != -1);

		[NonAction]
		public override ViewResult View(string viewName, object model)
		{
			if (IsAjax)
				ViewBag.Layout = "_PartialLayout";
			return base.View(viewName, model);
		}

		[NonAction]
		public IActionResult TryAjaxView<T>(string ajaxViewName, T model)
			=> IsAjax ? PartialView(ajaxViewName, model) : (IActionResult)View(model);

		public TResult ModelStateIsValid<T, TResult>(T model, Func<T, TResult> funcIfTrue, Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid)
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

		public Task<TResult> ModelStateIsValid<T, TResult>(T model, Func<T, Task<TResult>> funcIfTrue, Func<T, Task<TResult>> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid)
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

		public TResult ModelStateIsValid<T, TResult>(T model, Action<T> funcIfTrue, Func<T, TResult> next) where TResult : IActionResult
		{
			if (ModelState.IsValid)
				funcIfTrue(model);
			return next(model);
		}

		public TResult ModelStateIsValid<T, TResult>(T model, Func<T, bool> func, Func<T, TResult> funcIfTrue, Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid && func(model))
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

		public async Task<TResult> ModelStateIsValid<T, TResult>(T model, Func<T, Task<bool>> func, Func<T, TResult> funcIfTrue, Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid && await func(model))
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

	}

	public abstract class BaseController<TService, TModel> : BaseController<TService> where TService : BaseBeefeeService where TModel : class
	{
		protected readonly TModel Model;

		/// <summary>
		/// Гарант входящей модели
		/// </summary>
		protected BaseController(TService service, TModel model) : base(service)
		{
			Model = model;
			Model.As<TModel, IIdModel>(m => m.Id.HasNotNullArg("Id"));
			Model.As<TModel, IParentModel>(m => m.ParentId.HasNotNullArg("ParentId"));
		}
	}

}