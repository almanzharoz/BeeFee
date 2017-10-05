using BeeFee.ModeratorApp.Services;
using BeeFee.WebApplication.Controllers;
using BeeFee.Model.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeeFee.Model.Embed;
using BeeFee.WebApplication.Areas.Moderator.Models;

namespace BeeFee.WebApplication.Areas.Moderator.Controllers
{
	[Area("Moderator")]
	[Authorize(Roles = RoleNames.EventModerator)]
	public class EventController : BaseController<EventModeratorService>
	{
		public EventController(EventModeratorService service, CategoryService categoryService) : base(service, categoryService)
		{
		}

		public IActionResult Index(int page=0)
			=> View(Service.GetEvents(page, 20));

		[HttpGet]
		public IActionResult Moderate(string id, string companyId)
			=> View(new ModerateEventEditModel(Service.GetEvent(id, companyId)));

		[HttpPost]
		public IActionResult Moderate(ModerateEventEditModel model)
		{
			if (ModelState.IsValid)
			{
				// TODO: Добавить обработку ошибок
				Service.ModerateEvent(model.Id, model.CompanyId, model.Version, model.Comment, model.Result);
				return RedirectToAction("Index");
			}
			return View(model.Init(Service.GetEvent(model.Id, model.CompanyId)));
		}

		//public IActionResult Cancel(string id, string companyId, int version)
		//{
		//	Service.ModerateEvent(id, companyId, version, false);
		//	return RedirectToAction("Index");
		//}
	}
}
