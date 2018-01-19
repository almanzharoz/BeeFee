using System;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Antiforgery;
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
		private readonly IAntiforgery _antiforgery;

		public EventController(EventService service, CategoryService categoryService, ImagesService imagesService, EventRequestModel model, IAntiforgery antiforgery) : base(service, model)
		{
			_imagesService = imagesService;
			_categoryService = categoryService;
			_antiforgery = antiforgery;
		}

		#region Remove
		public ActionResult Remove()
		{
			Service.RemoveEvent(Model.Id, Model.ParentId, Model.Version);
			return RedirectToAction("Events", "Company");
		}
		#endregion

		#region Close
		public ActionResult Close()
		{
			Service.CloseEvent(Model.Id, Model.ParentId, Model.Version);
			return RedirectToAction("Events", "Company", new {id = Model.ParentId});
		}
		#endregion

		#region Edit

		[HttpGet]
		public Task<IActionResult> Edit()
		{
			_imagesService.GetAccess("/event", _antiforgery.GetAndStoreTokens(HttpContext).RequestToken, UserHost);
			return View("Edit",
				m => Service.GetEventAsync(m.Id, m.ParentId, m.Version),
				c => new EventEditModel(c /*.Fluent(x => _imagesService.GetAccessToFolder(x.Url, UserHost))*/,
					_categoryService.GetAllCategories<BaseCategoryProjection>()));
		}

		[HttpPost]
		public Task<IActionResult> Edit(EventEditModel model)
			=> ModelStateIsValid(model,
				async m =>
				{
					await _imagesService.Accept(new AcceptModel
					{
						TempPath = model.Cover,
						Images = new[] { new ImageSaveSetting("/event/cover", 368, 190), new ImageSaveSetting("/event", 1162, 600) }
					});
					return await Service.UpdateEventAsync(Model.Id, Model.ParentId, Model.Version, m.Name, m.Label, m.Url,
						m.Cover, m.Email, new EventDateTime(m.StartDateTime, m.FinishDateTime), new Address(m.City, m.Address),
						m.CategoryId);
				},
				m => RedirectToAction("EditDescription", "Event",
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
				m => Service.GetEventAsync(m.Id, m.ParentId, m.Version),
				c => new EventDescriptionModel(c/*.Fluent(x => _imagesService.GetAccessToFolder(x.Url, UserHost))*/));

		[HttpPost]
		public Task<IActionResult> EditDescription(EventDescriptionModel model)
			=> ModelStateIsValid(model,
				m => Service.UpdateEventDescriptionAsync(Model.Id, Model.ParentId, Model.Version, model.Html),
				m => RedirectToAction("Prices", "Event", new {area="Org", Model.Id, Model.ParentId, version=Model.Version+1}),
				m => View("Edit", m));

		#endregion

		#region Prices

		public async Task<IActionResult> Prices()
			=> View("Edit", new EventPricesModel((await Service.GetEventTransactionByIdAsync(Model.Id, Model.ParentId)).Prices));

		[HttpGet]
		public IActionResult AddPrice()
			=> View(new TicketPriceCreateModel(){Template = System.IO.File.ReadAllText("ticket.html") /*TODO: Брать не из файла*/});

		[HttpPost]
		public Task<IActionResult> AddPrice(TicketPriceCreateModel model)
			=> ModelStateIsValid(model,
				m => Service.AddEventTicketPriceAsync(Model.Id, Model.ParentId, m.Name, m.Description, m.Price, m.Count, m.Template),
				m => RedirectToAction("Prices", "Event", new { area = "Org", Model.Id, Model.ParentId, version = Model.Version }),
				View);

		#endregion

		#region Preview

		public async Task<IActionResult> Preview()
			=> new EventPreviewModel(await Service.GetPreviewEventAsync(Model.Id, Model.ParentId))
				.Convert(x => View(x.Event.State != EEventState.Open ? "Edit" : "Preview", x));
		
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