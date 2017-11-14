using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Areas.Org.Models.Event;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.Organizer)]
	public class EventController : BaseController<EventService, EventIdModel>
	{
		public EventController(EventService service, EventIdModel model) : base(service, model)
		{
		}

		#region Remove
		public ActionResult Remove()
		{
			return View();
		}
		#endregion

		#region Close
		public ActionResult Close()
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
		public ActionResult Edit(EventEditModel model)
		{
			return View();
		}

		[HttpGet]
		public ActionResult EditDescription()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EditDescription(EventDescriptionModel model)
		{
			return View();
		}

		[HttpGet]
		public ActionResult EditTicket(string tid)
		{
			return View();
		}

		[HttpPost]
		public ActionResult EditTicket(EventTicketModel model)
		{
			return View();
		}
		#endregion

		#region Preview
		public ActionResult Preview()
		{
			return View();
		}
		#endregion

		#region Publicate
		public ActionResult Publicate()
		{
			return View();
		}
		#endregion

		#region Registered
		public ActionResult Registered(RegisteredFilter model)
		{
			return View();
		}
		#endregion

	}
}