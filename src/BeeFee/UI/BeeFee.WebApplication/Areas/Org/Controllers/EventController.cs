using System;
using System.IO;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Services;
using BeeFee.WebApplication.Areas.Org.Models;
using BeeFee.WebApplication.Controllers;
using BeeFee.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using System.Linq;
using BeeFee.WebApplication.Models;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
    [Area("Org")]
    [Authorize(Roles = RoleNames.Organizer)]
    public class EventController : BaseController<EventService>
    {
        private readonly BeeFeeWebAppSettings _settings;
        private readonly ImagesService _imagesService;

        public EventController(EventService service, CategoryService categoryService, BeeFeeWebAppSettings settings) : base(service, categoryService)
        {
            _settings = settings;
            _imagesService = new ImagesService(_settings.ImagesUrl);
        }

        public IActionResult Index(string id)
        {
            ViewBag.CompanyId = id;
            return View(Service.GetMyEvents(id));
        }

		[HttpGet]
		public IActionResult Add(string companyId)
			=> View("Create", new CreateEventModel(
				Service.GetCompany<CompanyProjection>(companyId)
					.Fluent(x => _imagesService.GetAccessToFolder(x.Url, Request.Host.Host)),
				CategoryService.GetAllCategories<BaseCategoryProjection>())
			{
				StartDateTime = DateTime.Now,
				FinishDateTime = DateTime.Now.AddDays(1),
			});

		[HttpPost]
		public async Task<IActionResult> Add(CreateEventModel model)
		{
			if (ModelState.IsValid)
			{
				model.Url = model.Url.IfNull(model.Name, CommonHelper.UriTransliterate)
					.ThrowIf("/".ContainsExt,
						x => new InvalidOperationException("url contains \"/\"")); // <- не обращать внимания на эту строчку

				var eventId = model.Try(m =>
						Service.AddEvent(m.CompanyId,
							m.CategoryId,
							m.Name,
							m.Label,
							m.Url,
							m.Email,
							new EventDateTime(m.StartDateTime, m.FinishDateTime),
							new Address(m.City, m.Address),
							new[] {new TicketPrice("ticket", null, 0, 10)},
							m.Html,
							model.File != null && model.File.Length > 0 ? Path.GetFileName(model.File.FileName) : null))
					.Catch<EntityAccessException<Company>>((e, m) => ModelState.AddModelError("error", $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
					.Catch<ArgumentNullException>((e, m) => ModelState.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
					.Catch<ExistsUrlException<Event>>((e, m) => ModelState.AddModelError("Url", e.Message))
					.Use();

				ModelState[nameof(model.Url)].RawValue = model.Url; // hack
				if (eventId != null) // ошибок нет, событие сохранено
				{
					var company = Service.GetCompany<CompanyJoinProjection>(model.CompanyId);
					_imagesService.GetAccessToFolder(company.Url, Request.Host.Host);
					var r = await _imagesService.RegisterEvent(company.Url, model.Url,
						Request.Host.Host);
					if (model.File != null && model.File.Length > 0)
						await _imagesService.AddEventCover(company.Url, model.Url, Path.GetFileName(model.File.FileName),
							model.File.OpenReadStream());
					return RedirectToActionPermanent("Edit", new {id = eventId, companyId = company.Id, preview=true});
				}
			}
			return View("Create", model.Init(Service.GetCompany<CompanyJoinProjection>(model.CompanyId), CategoryService.GetAllCategories<BaseCategoryProjection>()));
		}

		[HttpGet]
        public IActionResult Edit(string id, string companyId, bool preview = false)
        {
            var @event = Service.GetEvent(id, companyId);
            if (@event == null || @event.State != EEventState.Created && @event.State != EEventState.NotModerated)
                return NotFound();
            _imagesService.GetAccessToFolder(@event.Parent.Url, Request.Host.Host);
			var model = new UpdateEventModel(@event, CategoryService.GetAllCategories<BaseCategoryProjection>());
			if (preview)
				model.SetPreviewWithStep(Service.GetPreviewEvent(id, companyId));
			else
				model.SetPreviewWithoutStep(Service.GetPreviewEvent(id, companyId));
			return View(model);
        }

        [HttpPost]
        public IActionResult Edit(UpdateEventModel model)
        {
			if (ModelState.IsValid)
			{
				if (model.Try(m => Service.UpdateEvent(
					m.Id,
					m.CompanyId,
					m.Version,
					m.Name,
					m.Label,
					m.Url,
					m.Cover,
					m.Email,
					new EventDateTime(m.StartDateTime, m.FinishDateTime),
					new Address(m.City, m.Address),
					m.CategoryId,
					//new[] { new TicketPrice() { Price = new Price(model.Price) } },
					null,
					m.Html))
					.Catch<EntityAccessException<Company>>((e, m) => ModelState.AddModelError("error", $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
					.Catch<ArgumentNullException>((e, m) => ModelState.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
					.Catch<ExistsUrlException<Event>>((e, m) => ModelState.AddModelError("Url", e.Message))
					.Catch<EventStateException>((e, m) => ModelState.AddModelError("error", $"Cобытие со статусом {e.State} нельзя изменить"))
					.Use())
				{
					ModelState[nameof(model.Version)].RawValue = model.Saved().Version; // hack
					model.SetPreviewWithStep(Service.GetPreviewEvent(model.Id, model.CompanyId));
				}
			}
			model.Init(Service.GetCompany<CompanyJoinProjection>(model.CompanyId), CategoryService.GetAllCategories<BaseCategoryProjection>());
            return View(model);
        }

        public IActionResult Preview(string id, string companyId)
            => Service.GetPreviewEvent(id, companyId).If(IsAjax, PartialView, x => (IActionResult)View(x));

        public IActionResult Remove(string id, string companyId, int version)
        {
			// TODO: добавить обработку ошибок
	        try
	        {
		        Service.RemoveEvent(id, companyId, version);
	        }
	        catch (RemoveEntityException)
	        {
		        return View("Error", new ErrorViewModel() {Message = "Произошла ошибка при удалении мероприятия"});
	        }
	        catch
	        {
		        return View("Error", new ErrorViewModel() {Message = "Произошла неизвестная ошибка"});
	        }
	        return RedirectToAction("Index", new { id = companyId });
        }

		public IActionResult Close(string id, string companyId, int version)
		{
			// TODO: добавить обработку ошибок
			try
			{
				Service.CloseEvent(id, companyId, version);
			}
			catch
			{
				return View("Error", new ErrorViewModel() { Message = "Произошла неизвестная ошибка" });
			}
			return RedirectToAction("Index", new { id = companyId });
		}

		public IActionResult ToModerate(string id, string companyId, int version)
        {
			// TODO: добавить обработку ошибок
	        try
	        {
		        Service.ToModerate(id, companyId, version);
	        }
	        catch (EntityAccessException<Company>)
	        {
		        return View("Error", new ErrorViewModel() {Message = "Произошла ошибка доступа"});
	        }
	        catch (ArgumentNullException)
	        {
		        return View("Error", new ErrorViewModel {Message = "Внутренняя ошибка сервера"});
	        }
	        catch
	        {
				return View("Error", new ErrorViewModel() { Message = "Произошла неизвестная ошибка" });
			}
			return RedirectToActionPermanent("Index", new { id = companyId });
        }
    }
}