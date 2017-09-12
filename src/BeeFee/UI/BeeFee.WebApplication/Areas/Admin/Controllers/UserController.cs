using BeeFee.AdminApp.Services;
using BeeFee.WebApplication.Areas.Admin.Models;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CategoryService = BeeFee.Model.Services.CategoryService;

namespace BeeFee.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
	public class UsersController : BaseController<UserService>
	{
		public UsersController(UserService service, CategoryService categoryService) : base(service, categoryService)
		{
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Add()
		{
			return PartialView(new AddUserModel());
		}

		[HttpPost]
		public IActionResult Add(AddUserModel addUserModel)
		{
			_service.AddUser(
				addUserModel.Email,
				addUserModel.Name,
				addUserModel.Roles);

			return RedirectToAction("Index");
		}

	}
}
