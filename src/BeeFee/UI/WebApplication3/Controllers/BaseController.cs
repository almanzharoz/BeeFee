using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.Model;
using Microsoft.AspNetCore.Mvc;
using Sharp7Func;
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

		protected string UserHost => Request.HttpContext.Connection.RemoteIpAddress.ToString();

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

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="model">Модель</param>
		/// <param name="func">Функция сервиса</param>
		/// <param name="funcIfTrue">Результат при успехе</param>
		/// <param name="funcIfFalse">Результат при ошибке</param>
		/// <returns></returns>
		public async Task<TResult> ModelStateIsValid<T, TResult>(T model, Func<T, Task<bool>> func, Func<T, TResult> funcIfTrue, Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid && await func(model))
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

		public async Task<TResult> ModelStateIsValid<T, TResult, TResultModel>(T model, 
			Func<T, Task<KeyValuePair<bool, TResultModel>>> func,
			Func<T, TResultModel, TResult> funcIfTrue, 
			Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			KeyValuePair<bool, TResultModel> r;
			if (ModelState.IsValid && (r = await func(model)).Key)
				return funcIfTrue(model, r.Value);
			return funcIfFalse(model);
		}

		public async Task<TResult> ModelStateIsValid<T, TResult>(T model,
			Func<T, Task<bool>> func,
			Func<T, TResult> funcIfTrue,
			Func<T, CatchCollection<T, Task<bool>>, CatchCollection<T, Task<bool>>> cathesFunc,
			Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			if (ModelState.IsValid && await cathesFunc(model, model.Try(func)).Use())
				return funcIfTrue(model);
			return funcIfFalse(model);
		}

		public async Task<TResult> ModelStateIsValid<T, TResult, TResultModel>(T model,
			Func<T, Task<KeyValuePair<bool, TResultModel>>> func,
			Func<T, TResultModel, Task<TResult>> funcIfTrue,
			Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			KeyValuePair<bool, TResultModel> r;
			if (ModelState.IsValid && (r = await func(model)).Key)
				return await funcIfTrue(model, r.Value);
			return funcIfFalse(model);
		}

		public async Task<TResult> ModelStateIsValid<T, TResult, TResultModel>(T model,
			Func<T, Task<KeyValuePair<bool, TResultModel>>> func,
			Func<T, TResultModel, Task<TResult>> funcIfTrue,
			Func<T, CatchCollection<T, Task<KeyValuePair<bool, TResultModel>>>, CatchCollection<T, Task<KeyValuePair<bool, TResultModel>>>> cathesFunc, 
			Func<T, TResult> funcIfFalse) where TResult : IActionResult
		{
			KeyValuePair<bool, TResultModel> r;
			if (ModelState.IsValid && (r = await cathesFunc(model, model.Try(func)).Use()).Key)
				return await funcIfTrue(model, r.Value);
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

		protected IActionResult View<TProjection, TViewModel>(string viewName, Func<TModel, TProjection> getFunc, Func<TProjection, TViewModel> createModel)
		{
			var p = getFunc(Model);
			if (p.IsNull())
				return NotFound();
			return View(viewName, createModel(p));
		}

		protected async Task<IActionResult> View<TProjection, TViewModel>(string viewName, Func<TModel, Task<TProjection>> getFunc, Func<TProjection, TViewModel> createModel)
		{
			var p = await getFunc(Model);
			if (p.IsNull())
				return NotFound();
			return View(viewName, createModel(p));
		}
	}

}