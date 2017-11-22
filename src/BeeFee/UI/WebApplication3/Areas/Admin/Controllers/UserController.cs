using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.AdminApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Admin.Models;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class UserController : BaseController<UserService, UserRequestModel>
    {
		public UserController(UserService service, UserRequestModel model) : base(service, model)
		{
		}

        public IActionResult Index()
        {
            return View();
        }

	}
}