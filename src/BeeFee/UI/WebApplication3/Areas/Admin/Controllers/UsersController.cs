using BeeFee.AdminApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class UsersController : BaseController<UserService>
    {
		public UsersController(UserService service) : base(service)
		{
		}

		public IActionResult Index()
        {
            return View();
        }

	}
}