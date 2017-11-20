using BeeFee.ModeratorApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Moderator.Models.Events;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Moderator.Controllers
{
    public class EventsController : BaseController<EventModeratorService>
    {
		public EventsController(EventModeratorService service) : base(service)
		{
		}

		/// <summary>
		/// Список мероприятий, ожидающих публикации
		/// </summary>
		public IActionResult Index(EventsFilter model)
			=> View(model.Load(Service.GetEvents(model.Page, model.Size)));

	}
}