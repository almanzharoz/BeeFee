using System;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
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
			=> View(new AddEventEditModel(
				Service.GetCompany<CompanyProjection>(companyId)
					.Fluent(x => _imagesService.GetAccessToFolder(x.Url, Request.Host.Host)),
				CategoryService.GetAllCategories<BaseCategoryProjection>())
			{
				StartDateTime = DateTime.Now,
				FinishDateTime = DateTime.Now.AddDays(1)
			});

		[HttpPost]
		public async Task<IActionResult> Add(AddEventEditModel model)
		{
			if (ModelState.IsValid)
			{
				model.Url = model.Url.IfNull(model.Name, CommonHelper.UriTransliterate)
					.ThrowIf("/".ContainsExt, x => new InvalidOperationException("url contains \"/\""));

				if (Service.AddEvent(model.CompanyId,
					model.CategoryId,
					model.Name,
					model.Label,
					model.Url,
					model.Email,
					new EventDateTime(model.StartDateTime, model.FinishDateTime),
					new Address(model.City, model.Address),
					new[] {new TicketPrice("ticket", null, 0, 10)},
					model.Html,
					model.File != null && model.File.Length > 0 ? model.File.FileName : null
				))
				{
					var company = Service.GetCompany<CompanyJoinProjection>(model.CompanyId);
					await _imagesService.RegisterEvent(company.Url, model.Url, Request.Host.Host);
					if (model.File != null && model.File.Length > 0)
						await _imagesService.AddEventCover(company.Url, model.Url, model.File.FileName, model.File.OpenReadStream());
					return RedirectToAction("Index", new {id = model.CompanyId});
				}
				ModelState.AddModelError("error", "Event dont save");
			}
			return View(model.Init(CategoryService.GetAllCategories<BaseCategoryProjection>()));
		}

		[HttpGet]
        public IActionResult Edit(string id, string companyId)
        {
            var @event = Service.GetEvent(id, companyId);
            if (@event == null || @event.State != EEventState.Created && @event.State != EEventState.NotModerated)
                return NotFound();
			_imagesService.GetAccessToFolder(@event.Parent.Url, Request.Host.Host);

			return View(new EventEditModel(@event, CategoryService.GetAllCategories<BaseCategoryProjection>()));
        }

        [HttpPost]
        public IActionResult Edit(EventEditModel model)
        {
            if (ModelState.IsValid)
            {
                Service.UpdateEvent(
                 model.Id,
                 model.CompanyId,
                 model.Version,
                 model.Name,
                 model.Label,
                 model.Url,
                 model.Cover,
                 model.Email,
                 new EventDateTime(model.StartDateTime, model.FinishDateTime),
                 new Address(model.City, model.Address),
                 model.CategoryId,
                 //new[] { new TicketPrice() { Price = new Price(model.Price) } },
                 null,
                 model.Html);
                return RedirectToAction("Index", new { id = model.CompanyId });
            }
            model.Init(CategoryService.GetAllCategories<BaseCategoryProjection>());
            return View(model);
        }

		public IActionResult Preview(string id, string companyId)
			=> Service.GetPreviewEvent(id, companyId).If(IsAjax, PartialView, x => (IActionResult)View(x));

        public IActionResult Remove(string id, string companyId, int version)
        {
            // TODO: добавить обработку ошибок
            Service.RemoveEvent(id, companyId, version);
            return RedirectToAction("Index", new { id = companyId });
        }

		public IActionResult ToModerate(string id, string companyId, int version)
		{
			Service.ToModerate(id, companyId, version);
			return RedirectToActionPermanent("Index", new { id = companyId });
		}
    }
}