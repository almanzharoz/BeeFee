using BeeFee.AdminApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Admin.Models;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
	public class CategoryController : BaseController<CategoryService, CategoryRequestModel>
	{
		public CategoryController(CategoryService service, CategoryRequestModel model) : base(service, model)
		{
		}

		public IActionResult Index()
		{
			return
			View();
		}

	}
}