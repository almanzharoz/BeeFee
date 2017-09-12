using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeeFee.Model;
using BeeFee.Model.Services;

namespace BeeFee.WebApplication.Controllers
{
    public abstract class BaseController<TService> : Controller where TService : BaseBeefeeService
	{
		protected readonly TService _service;
		protected readonly CategoryService _categoryService;

		protected BaseController(TService service, CategoryService categoryService)
		{
			_service = service;
			_categoryService = categoryService;
		}
    }
}