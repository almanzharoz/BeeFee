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
    public class EventController : BaseController<EventService, EventIdModel>
    {
		private readonly BeeFeeWebAppSettings _settings;

		public EventController(EventService service, BeeFeeWebAppSettings settings, EventIdModel model) : base(service, model)
		{
			_settings = settings;
		}

		public async Task<IActionResult> Index()
			=> (await Service.GetEventByUrl(Model.ParentId, Model.Id)).IfNotNull(e =>
					View(new EventPageModel(e, /* TODO: Доставать юзера из БД */ User.Identity.Name,
						User.Claims.Where(c => c.Type.Equals(ClaimTypes.Email, StringComparison.Ordinal)).Select(c => c.Value)
							.FirstOrDefault(), "")),
				() => (IActionResult) NotFound());

		#region Register

		[HttpPost]
		public Task<IActionResult> Register(EventPageModel model)
			=> ModelStateIsValid(model, async m =>
					(await Service.GetEventByUrl(Model.ParentId, Model.Id)).Convert(e =>
						Service.RegisterToEvent(Model.ParentId, Model.Id, m.Email, m.Name, m.Phone, e.Prices.First().Id,
							String.Concat(_settings.ImagesUrl, "/min/", e.Parent.Url, "/", e.Url, "/1162x600/") /* TODO: Выпилить */)),
				m => (IActionResult) View(),
				m => View());

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