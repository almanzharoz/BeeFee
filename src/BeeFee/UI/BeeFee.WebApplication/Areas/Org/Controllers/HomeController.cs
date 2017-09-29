using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeeFee.Model.Embed;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.Organizer)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}