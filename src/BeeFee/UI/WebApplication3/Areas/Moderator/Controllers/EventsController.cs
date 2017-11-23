using System.Threading.Tasks;
using BeeFee.ModeratorApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Moderator.Models.Events;
using WebApplication3.Controllers;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Areas.Moderator.Controllers
{
	[Area("Moderator")]
	[Authorize(Roles = RoleNames.EventModerator)]
	public class EventsController : BaseController<EventModeratorService>
    {
		public EventsController(EventModeratorService service) : base(service)
		{
		}

		/// <summary>
		/// Список мероприятий, ожидающих публикации
		/// </summary>
		public async Task<IActionResult> Index(EventsFilter model)
			=> View(model.Load(await Service.GetEvents(model.Page, model.Size)));

	}
}