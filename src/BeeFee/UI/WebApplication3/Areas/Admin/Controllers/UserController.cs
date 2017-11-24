using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Admin.Models;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = RoleNames.Admin)]
	public class UserController : BaseController<UserService, UserRequestModel>
    {
		public UserController(UserService service, UserRequestModel model) : base(service, model)
		{
		}

		public bool ChangeRole(string id, EUserRole role)
			=> Service.ChangeRole(id, role);


	}
}