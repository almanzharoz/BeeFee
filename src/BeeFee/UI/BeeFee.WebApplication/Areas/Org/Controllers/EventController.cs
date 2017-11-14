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
using BeeFee.WebApplication.Infrastructure.ViewRendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
    [Area("Org")]
    [Authorize(Roles = RoleNames.Organizer)]
    public class EventController : BaseController<EventService>
    {
        private readonly BeeFeeWebAppSettings _settings;
        private readonly ImagesService _imagesService;

        public EventController(EventService service, CategoryService categoryService, BeeFeeWebAppSettings settings, ViewRenderService viewRenderService) : base(service, categoryService)
        {
            _settings = settings;
            _imagesService = new ImagesService(_settings.ImagesUrl);
        }

        public IActionResult Index(string id)
		{
			if (String.IsNullOrEmpty(id))
				id = Service.GetCompany<CompanyProjection>(id).Id;
            ViewBag.CompanyId = id;
            return View(Service.GetMyEvents(id));
        }

		[HttpGet]
		public IActionResult Add(string companyId)
			=> View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/General.cshtml", new CreateOrUpdateEventGeneralStepModel(Service
					.GetCompany<CompanyProjection>(companyId)
					.Fluent(x => _imagesService.GetAccessToFolder(x.Url, Request.HttpContext.Connection.RemoteIpAddress.ToString()))
					.Id, CategoryService.GetAllCategories<BaseCategoryProjection>())
				{
					StartDateTime = DateTime.Now,
					FinishDateTime = DateTime.Now.AddDays(1),
				}
			);

        [HttpGet]
        public IActionResult Edit(string id, string companyId)
        {
            var @event = Service.GetEvent(id, companyId);
            if (@event == null || @event.State != EEventState.Created && @event.State != EEventState.NotModerated)
                return NotFound();
            _imagesService.GetAccessToFolder(@event.Parent.Url, @event.Url, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/General.cshtml", new CreateOrUpdateEventGeneralStepModel(@event, CategoryService.GetAllCategories<BaseCategoryProjection>()));
        }


        [HttpPost]
		[RequestSizeLimit(5000000)]
        public async Task<IActionResult> EventGeneralSettingsStep(CreateOrUpdateEventGeneralStepModel model)
        {
            if (model.StartDateTime > model.FinishDateTime)
            {
                ModelState.AddModelError(nameof(model.StartDateTime),
                    $"Дата начала позднее даты окончания");
            }
            if (model.IsNew && model.File == null || !model.IsNew && string.IsNullOrEmpty(model.Cover))
            {
                ModelState.AddModelError(nameof(model.File),
                    $"Необходимо выбрать изображение");
            }
            if (ModelState.IsValid)
            {
                var allOk = false;
                if (model.IsNew)//если модель для нового события то создаём его
                {
                    model.Url = model.Url.IfNull(model.Name, CommonHelper.UriTransliterate)
                        .ThrowIf("/".ContainsExt,
                            x => new InvalidOperationException(
                                "url contains \"/\"")); // <- не обращать внимания на эту строчку
                    var eventId = model.Try(m =>
                            Service.AddEvent(m.CompanyId,
                                m.CategoryId,
                                m.Name,
                                m.Label,
                                m.Url,
                                m.Email,
                                new EventDateTime(m.StartDateTime, m.FinishDateTime),
                                new Address(m.City, m.Address),
                                new[] { new TicketPrice("ticket", null, 0, 10) },
                                "",
                                model.File != null && model.File.Length > 0
                                    ? Path.GetFileName(model.File.FileName)
                                    : null))
                        .Catch<EntityAccessException<Company>>((e, m) => ModelState.AddModelError("error",
                            $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
                        .Catch<ArgumentNullException>((e, m) =>
                            ModelState.AddModelError("error",
                                $"Не указан или не найден аргумент \"{e.ParamName}\""))
                        .Catch<ExistsUrlException<Event>>((e, m) => ModelState.AddModelError("Url", e.Message))
                        .Use();

                    if (eventId != null) // ошибок нет, событие сохранено
                    {
                        var company = Service.GetCompany<CompanyJoinProjection>(model.CompanyId);
                        var r = await _imagesService.RegisterEvent(company.Url, model.Url, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        if (model.File != null && model.File.Length > 0)
                            await _imagesService.AddEventCover(company.Url, model.Url,
                                Path.GetFileName(model.File.FileName),
                                model.File.OpenReadStream());
                        model.Id = eventId;
                        allOk = true;
                    }
                }
                else//значит мы должны обновить существующее событие
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
                                    null))
                                .Catch<EntityAccessException<Company>>((e, m) => ModelState.AddModelError("error",
                                    $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
                                .Catch<ArgumentNullException>((e, m) =>
                                    ModelState.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
                                .Catch<ExistsUrlException<Event>>((e, m) => ModelState.AddModelError("Url", e.Message))
                                .Catch<EventStateException>((e, m) =>
                                    ModelState.AddModelError("error", $"Cобытие со статусом {e.State} нельзя изменить"))
                                .Use())
                    {
                        allOk = true;
                    }
                }
                if (allOk)
                    return RedirectToActionPermanent("EditDescriptionStep",
                        new { id = model.Id, companyId = model.CompanyId });
            }
            return View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/General.cshtml", model.Init(CategoryService.GetAllCategories<BaseCategoryProjection>()));
        }

        [HttpGet]
        public IActionResult EditDescriptionStep(string id, string companyId)
        {
            var @event = Service.GetEvent(id, companyId);
            if (@event == null || @event.State != EEventState.Created && @event.State != EEventState.NotModerated)
                return NotFound();
            _imagesService.GetAccessToFolder(@event.Parent.Url, @event.Url, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/Description.cshtml", new CreateOrUpdateEventDescriptionStepModel(@event));
        }

        [HttpPost]
        public IActionResult EditDescriptionStep(CreateOrUpdateEventDescriptionStepModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Try(m => Service.UpdateEvent(
                        m.Id,
                        m.CompanyId,
                        m.Version,
                        m.Html))
                    .Catch<EntityAccessException<Company>>((e, m) => ModelState.AddModelError("error",
                        $"Невозможно получить доступ к указанной компании (Company={e.Id}, User={e.User})"))
                    .Catch<ArgumentNullException>((e, m) =>
                        ModelState.AddModelError("error", $"Не указан или не найден аргумент \"{e.ParamName}\""))
                    .Catch<ExistsUrlException<Event>>((e, m) => ModelState.AddModelError("Url", e.Message))
                    .Catch<EventStateException>((e, m) =>
                        ModelState.AddModelError("error", $"Cобытие со статусом {e.State} нельзя изменить"))
                    .Use())
                {
                    return RedirectToActionPermanent("PreviewStep",
                        new { id = model.Id, companyId = model.CompanyId });
                }
            }
            return View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/Description.cshtml", model);
        }

        [HttpGet]
        public IActionResult PreviewStep(string id, string companyId)
        {
            var @event = Service.GetEvent(id, companyId);
            if (@event == null || @event.State != EEventState.Created && @event.State != EEventState.NotModerated)
                return NotFound();
            return View("~/Areas/Org/Views/Event/CreateOrUpdateEvent/Preview.cshtml", new CreateOrUpdateEventPreviewStepModel(Service.GetPreviewEvent(id, companyId), @event.Version));
        }

        public IActionResult Preview(string id, string companyId)
            => Service.GetPreviewEvent(id, companyId).If(IsAjax, PartialView, x => (IActionResult)View(x));

        public IActionResult Remove(string id, string companyId, int version)
        {
            // TODO: добавить обработку ошибок
            Service.RemoveEvent(id, companyId, version);
            return RedirectToAction("Index", new { id = companyId });
        }

        public IActionResult Close(string id, string companyId, int version)
        {
            // TODO: добавить обработку ошибок
            Service.CloseEvent(id, companyId, version);
            return RedirectToAction("Index", new { id = companyId });
        }

        public IActionResult ToModerate(string id, string companyId, int version)
        {
            // TODO: добавить обработку ошибок
            Service.ToModerate(id, companyId, version);
            return RedirectToActionPermanent("Index", new { id = companyId });
        }

		public IActionResult Registered(string id, string companyId, int page=0, int limit = 10)
			=> View(Service.GetRegisteredUsers(id, companyId, page, limit));
	}
}