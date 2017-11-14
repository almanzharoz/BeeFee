using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Org.Models.Company;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.Organizer)]
    public class CompanyController : BaseController<CompanyService, CompanyIdModel>
    {
		protected CompanyController(CompanyService service, CompanyIdModel model) : base(service, model)
		{
		}

		#region Remove
		[Authorize(Roles = RoleNames.MultiOrganizer)]
		public ActionResult Remove()
		{
			return View();
		}
		#endregion

		#region Edit
		[HttpGet]
		public ActionResult Edit()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Edit(CompanyEditModel model)
		{
			return View();
		}
		#endregion

		#region Events
		public ActionResult Events(EventsFilter model)
		{
			return View();
		}
		#endregion

		#region Create Event
		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(CreateEventModel model)
		{
			return View();
		}
		#endregion
	}
}