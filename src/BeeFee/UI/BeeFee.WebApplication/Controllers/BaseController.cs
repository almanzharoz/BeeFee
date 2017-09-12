using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BeeFee.Model;
using BeeFee.Model.Services;

namespace BeeFee.WebApplication.Controllers
{
    public abstract class BaseController<TService> : Controller where TService : BaseBeefeeService
	{
		protected readonly TService Service;
		protected readonly CategoryService CategoryService;

		protected BaseController(TService service, CategoryService categoryService)
		{
			Service = service;
			CategoryService = categoryService;
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

	}
}