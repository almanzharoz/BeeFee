using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Embed;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Infrastructure.Services;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.WebApplication.Controllers
{
    public class EventController : BaseController<EventService>
    {
        private readonly EventUIService _eventUIService;
        public EventController(EventService service, CategoryService categoryService, EventUIService eventUiService) : base(service, categoryService)
        {
            _eventUIService = eventUiService;
        }

        //public IActionResult Index()
        //      {
        //          return View();
        //      }

        //public IActionResult Events()
        //{
        //    var model = new EventFilterViewModel();
        //    var nowTime = DateTime.UtcNow;
        //    model.StartDate = new DateTime(nowTime.Year, nowTime.Month, 1);
        //    model.EndDate = new DateTime(nowTime.Year, nowTime.Month + 1, 1).AddDays(-1);
        //    model.Cities = _service.GetAllCities().ToList();
        //    model.Categories = _categoryService.GetAllCategories<CategoryProjection>().Select(c => new SelectListItem() { Value = c.Id, Text = c.Name }).ToList();
        //    return View(model);
        //}

        [Route("/event/{cid}/{id}")]
        public async Task<IActionResult> Event(string id, string cid, int? r)
        //=> id.IfNotNull(x => _service.GetEventByUrl(id)
        //  .IfNotNull<EventProjection, IActionResult>(View, NotFound),
        // () => Task.FromResult((IActionResult)NotFound())); // рабочий пример, когда не надо юзать fluent :)
        {
            var @event = await Service.GetEventByUrl(cid, id);
            if (@event == null)
                return NotFound();
            return View(new EventPageModel(@event, User.Identity.Name, User.Claims.Where(c => c.Type.Equals(ClaimTypes.Email, StringComparison.Ordinal)).Select(c => c.Value).FirstOrDefault(), "", @event.Prices.First().Left>0 && @event.State != EEventState.Close /*TODO: Добавить проверки дат*/ ? r : 3));
        }

        [Route("/event/{cid}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterToEvent([Bind("Name", "Email", "Phone")]EventPageModel model, string id, string cid)
        {
            var @event = await Service.GetEventByUrl(cid, id);
            if (@event == null)
                return NotFound();
			if (ModelState.IsValid)
            {
                if (Service.RegisterToEvent(@event.Id, @event.Parent.Id, model.Email, model.Name, model.Phone,
                    @event.Prices.First().Id))
                {
                    return RedirectToAction("Event", new { id = @event.Url, cid = @event.Parent.Url, r = 0 });
                }
                return RedirectToAction("Event", new { id = @event.Url, cid = @event.Parent.Url, r = 1 });
            }
            model.Event = @event;
            return View("Event", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadEvents(LoadEventsRequest request)
        {
            var result = await _eventUIService.GetEventsListRenderAsync(request);
            return Json(new { allLoaded = result.AllLoaded, html = result.Html });
        }

    }
}