using BeeFee.ModeratorApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Moderator.Models;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Moderator.Controllers
{
    public class EventController : BaseController<EventModeratorService, EventIdModel>
    {
		public EventController(EventModeratorService service, EventIdModel model) : base(service, model)
		{
		}

		#region Reject
		[HttpGet]
		public ActionResult Reject()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Reject(object model)
		{
			return View();
		}
		#endregion

		#region Return
		[HttpGet]
		public ActionResult Return()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Return(object model)
		{
			return View();
		}
		#endregion

		#region Publicate
		[HttpPost]
		public ActionResult Publicate(object model)
		{
			return View();
		}
		#endregion

	}
}