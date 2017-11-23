using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Models;
using WebApplication3.Models.Event;

namespace WebApplication3.Controllers
{
    public class EventController : BaseController<EventService, EventRequestModel>
    {
		private readonly BeeFeeWebAppSettings _settings;

		public EventController(EventService service, BeeFeeWebAppSettings settings, EventRequestModel model) : base(service, model)
		{
			_settings = settings;
		}

		[Route("event/{parentid}/{id}")]
		[HttpGet]
		public Task<IActionResult> Index()
			=> View("Index",
				m => Service.GetEventByUrl(m.ParentId, m.Id),
				async e => new EventPageModel(e,
					await Service.GetEventTransaction(e.Id),
					User.Identity.Name /* TODO: Доставать юзера из БД */,
					User.Claims.Where(c => c.Type.Equals(ClaimTypes.Email, StringComparison.Ordinal)).Select(c => c.Value)
						.FirstOrDefault(), "", Service.CanRegister(e.Id, e.Parent.Id, HttpContext.Session?.Id) ? null : (bool?)false));

		#region Register

		[HttpPost]
		public Task<IActionResult> Register(EventPageModel model)
			=> ModelStateIsValid(model, async m =>
					await (await Service.GetEventByUrl(Model.ParentId, Model.Id)).Convert(e =>
						Service.RegisterToEventAsync(e.Id, e.Parent.Id, m.Email, m.Name, m.Phone, m.TicketId,
							String.Concat(_settings.ImagesUrl, "/min/", e.Parent.Url, "/", e.Url, "/1162x600/") /* TODO: Выпилить */, HttpContext.Session?.Id)),
				m => View("Index", M => Service.GetEventByUrl(M.ParentId, M.Id),
					async e => new EventPageModel(e, await Service.GetEventTransaction(e.Id), true)),
				m => View("Index", M => Service.GetEventByUrl(M.ParentId, M.Id),
					async e => new EventPageModel(e, await Service.GetEventTransaction(e.Id), false)));

		#endregion

		#region Contact
		[HttpGet]
		public ActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Contact(object model)
		{
			return View();
		}
		#endregion

	}
}