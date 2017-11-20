using System.Threading.Tasks;
using BeeFee.ModeratorApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Moderator.Models;
using WebApplication3.Areas.Moderator.Models.Event;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Moderator.Controllers
{
    public class EventController : BaseController<EventModeratorService, EventRequestModel>
    {
		public EventController(EventModeratorService service, EventRequestModel model) : base(service, model)
		{
		}

		[HttpGet]
		public IActionResult Index()
			=> View(new EventModerateModel(Service.GetEvent(Model.Id, Model.ParentId)));

		#region Moderate

		[HttpPost]
		public Task<IActionResult> Moderate(EventModerateModel model)
			=> ModelStateIsValid(model, 
				m => Service.ModerateEvent(Model.Id, Model.ParentId, Model.Version, m.Comment, m.Moderate),
				m => View(m),
				m => View("Index", m.Init(Service.GetEvent(Model.Id, Model.ParentId)))); //TODO: Обработка ошибок
		#endregion

	}
}