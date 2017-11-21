using System;
using System.IO;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Areas.Org.Models.Company;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.Organizer)]
    public class CompanyController : BaseController<CompanyService, CompanyRequestModel>
    {
		private readonly ImagesService _imagesService;
		private readonly EventService _eventService;
		private readonly CategoryService _categoryService;

		public CompanyController(CompanyService service, EventService eventService, CategoryService categoryService, ImagesService imagesService, CompanyRequestModel model) : base(service, model)
		{
			_imagesService = imagesService;
			_eventService = eventService;
			_categoryService = categoryService;
		}

		#region Remove
		[Authorize(Roles = RoleNames.MultiOrganizer)]
		public ActionResult Remove()
		{
			Service.RemoveCompany(Model.Id, Model.Version);
			return RedirectToActionPermanent("Index", "Companies");
		}
		#endregion

		#region Edit

		[HttpGet]
		public Task<IActionResult> Edit()
			=> View("Edit",
				m => Service.GetCompanyAsync(m.Id),
				c => new CompanyEditModel(c.Fluent(x => _imagesService.GetAccessToFolder(x.Url, UserHost))));

		[HttpPost]
		public Task<IActionResult> Edit(CompanyEditModel model)
			=> ModelStateIsValid(model,
				m => Service.EditCompanyAsync(Model.Id, Model.Version, model.Name, model.Url, model.Email, model.Logo),
				m => User.IsInRole(RoleNames.MultiOrganizer)
					? RedirectToActionPermanent("Index", "Companies")
					: RedirectToActionPermanent("Events", "Company", new {area = "Org", Model.Id}),
				m => View(m));
		#endregion

		#region Events
		public async Task<IActionResult> Events(EventsFilter model)
			=> View(model.Load(await _eventService.GetMyEventsAsync(Model.Id, model.Page, model.Size)));
		#endregion

		#region Create Event

		[HttpGet]
		public ActionResult CreateEvent()
			=> View(new EventCreateModel(_categoryService.GetAllCategories<BaseCategoryProjection>()));

		[HttpPost]
		public Task<IActionResult> CreateEvent(EventCreateModel model)
			=> ModelStateIsValid(model,
				// пробуем добавить мероприятие в БД
				m => _eventService.AddEventAsync(Model.Id, m.CategoryId, m.Name, m.Label, m.Url, m.Email,
					new EventDateTime(m.StartDateTime, m.FinishDateTime), new Address(m.City, m.Address), m.Cover),
				// если удалось добавить в БД
				async (m, r) =>
				{
					var companyTask = _eventService.GetCompanyAsync<CompanyJoinProjection>(Model.Id);
					var newEventTask = _eventService.GetEventAsync(r, Model.Id);
					var company = await companyTask;
					var newEvent = await newEventTask;
					var e = await _imagesService.RegisterEvent(company.Url, newEvent.Url, UserHost);
					if (e && model.File != null && model.File.Length > 0)
						await _imagesService.AddEventCover(company.Url, newEvent.Url,
							Path.GetFileName(model.File.FileName),
							model.File.OpenReadStream());
					return (IActionResult) RedirectToActionPermanent("EditDescription", "Event", new {area = "Org", id = r});
				},
				// обработка исключений
				(m, ms, c) => c
					.Catch<EntityAccessException<Company>>((e, r) =>
						ms.AddModelError("error", $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
					.Catch<ArgumentNullException>((e, r) =>
						ms.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
					.Catch<ExistsUrlException<Event>>((e, r) => 
						ms.AddModelError("Url", e.Message)),
				// если модель не валидна или не удалось добавить в БД
				m => View(m.Init(_categoryService.GetAllCategories<BaseCategoryProjection>())));

		#endregion
	}
}