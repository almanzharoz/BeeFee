using System;
using System.IO;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
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
	public class EventController : BaseController<EventService, EventRequestModel>
	{
		private readonly ImagesService _imagesService;
		private readonly CategoryService _categoryService;

		public EventController(EventService service, CategoryService categoryService, ImagesService imagesService, EventRequestModel model) : base(service, model)
		{
			_imagesService = imagesService;
			_categoryService = categoryService;
		}

		#region Remove
		public ActionResult Remove()
		{
			Service.RemoveEvent(Model.Id, Model.ParentId, Model.Version);
			return RedirectToActionPermanent("Events", "Company");
		}
		#endregion

		#region Close
		public ActionResult Close()
		{
			Service.CloseEvent(Model.Id, Model.ParentId, Model.Version);
			return RedirectToActionPermanent("Events", "Company");
		}
		#endregion

		#region Edit
		[HttpGet]
		public Task<IActionResult> Edit()
			=> View("Edit",
				m => Service.GetEventAsync(m.Id, m.ParentId),
				c => new EventEditModel(c.Fluent(x => _imagesService.GetAccessToFolder(x.Url, UserHost)), _categoryService.GetAllCategories<BaseCategoryProjection>()));

		[HttpPost]
		public Task<IActionResult> Edit(EventEditModel model)
			=> ModelStateIsValid(model,
				async m => await Service.UpdateEventAsync(Model.Id, Model.ParentId, Model.Version, m.Name, m.Label, m.Url,
					await m.Cover.If(x => model.File != null && model.File.Length > 0,
						x => _imagesService.AddEventCover(Model.ParentId, Model.Id, x, model.File.OpenReadStream())),
					m.Email, new EventDateTime(m.StartDateTime, m.FinishDateTime), new Address(m.City, m.Address), m.CategoryId),
				m => RedirectToActionPermanent("EditDescription", "Event",
					new {area = "Org", Model.Id, Model.ParentId, version = Model.Version + 1}),
				(m, c) => c
					.Catch<EntityAccessException<Company>>((e, r) =>
						ModelState.AddModelError("error",
							$"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
					.Catch<ArgumentNullException>((e, r) =>
						ModelState.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
					.Catch<ExistsUrlException<Event>>((e, r) =>
						ModelState.AddModelError("Url", e.Message)),
				View);

		[HttpGet]
		public Task<IActionResult> EditDescription()
			=> View("Edit",
				m => Service.GetEventAsync(m.Id, m.ParentId),
				c => new EventDescriptionModel(c.Fluent(x => _imagesService.GetAccessToFolder(x.Url, UserHost))));

		[HttpPost]
		public Task<IActionResult> EditDescription(EventDescriptionModel model)
			=> ModelStateIsValid(model,
				m => Service.UpdateEventDescriptionAsync(Model.Id, Model.ParentId, Model.Version, model.Html),
				m => RedirectToActionPermanent("Prices", "Event", new {area="Org", Model.Id, Model.ParentId, version=Model.Version+1}),
				View);

		#endregion

		#region Prices

		public async Task<IActionResult> Prices()
			=> View((await Service.GetEventTransactionByIdAsync(Model.Id, Model.ParentId)).Prices);

		[HttpGet]
		public IActionResult AddPrice()
			=> View(new TicketPriceCreateModel());

		[HttpPost]
		public Task<IActionResult> AddPrice(TicketPriceCreateModel model)
			=> ModelStateIsValid(model,
				m => Service.AddEventTicketPriceAsync(Model.Id, Model.ParentId, m.Name, m.Description, m.Price, m.Count, m.Template),
				m => RedirectToActionPermanent("Prices", "Event", new { area = "Org", Model.Id, Model.ParentId }),
				View);

		#endregion

		#region Preview

		public async Task<IActionResult> Preview()
			=> View(await Service.GetPreviewEventAsync(Model.Id, Model.ParentId));
		
		#endregion

		#region Publicate
		public ActionResult Publicate()
		{
			Service.ToModerate(Model.Id, Model.ParentId, Model.Version);
			return View();
		}
		#endregion

		#region Registered
		public async Task<IActionResult> Registered(RegisteredFilter model)
			=> View(model.Load(await Service.GetRegisteredUsersAsync(Model.Id, Model.ParentId, model.Page, model.Size)));
		#endregion

	}
}