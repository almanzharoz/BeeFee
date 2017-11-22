using BeeFee.AdminApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class CategoriesController : BaseController<CategoryService>
    {
		public CategoriesController(CategoryService service) : base(service)
		{
		}

		public IActionResult Index()
        {
            return View();
        }

	}
}