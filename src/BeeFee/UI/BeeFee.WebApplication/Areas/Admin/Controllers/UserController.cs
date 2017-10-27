using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using BeeFee.WebApplication.Areas.Admin.Models;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CategoryService = BeeFee.Model.Services.CategoryService;

namespace BeeFee.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = RoleNames.Admin)]
	public class UsersController : BaseController<UserService>
	{
		public UsersController(UserService service, CategoryService categoryService) : base(service, categoryService)
		{
		}

		public IActionResult Index(int page = 0)
		{
			return View(Service.GetUsers(page));
		}

		public bool ChangeRole(string id, EUserRole role)
			=> Service.ChangeRole(id, role);

		[HttpGet]
		public IActionResult Add()
		{
			return PartialView(new AddUserModel());
		}

		[HttpPost]
		public IActionResult Add(AddUserModel addUserModel)
		{
			Service.AddUser(
				addUserModel.Email,
				addUserModel.Name,
				addUserModel.Roles);

			return RedirectToAction("Index");
		}

	}
}
