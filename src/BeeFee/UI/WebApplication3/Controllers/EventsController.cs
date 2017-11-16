using BeeFee.ClientApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models.Events;

namespace WebApplication3.Controllers
{
	public class EventsController : BaseController<EventService>
	{
		public EventsController(EventService service) : base(service)
		{
		}

		public IActionResult Index(EventsFilter filterModel)
			=> View(Service.SearchEvents());

	}
}