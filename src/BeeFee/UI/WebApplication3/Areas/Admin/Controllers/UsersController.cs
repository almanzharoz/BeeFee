using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Admin.Models.Users;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = RoleNames.Admin)]
	public class UsersController : BaseController<UserService>
    {
		public UsersController(UserService service) : base(service)
		{
		}

		public IActionResult Index(UsersFilter model)
			=> View(model.Load(Service.GetUsers(model.Page, model.Size)));

	}
}