using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Infrastructure.Services;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Controllers
{
    public class HomeController : BaseController<EventService>
    {
        private readonly EventUIService _eventUIService;
        public HomeController(EventService service, CategoryService categoryService, EventUIService eventUiService) : base(service, categoryService)
        {
            _eventUIService = eventUiService;
        }

        public async Task<IActionResult> Index(LoadEventsRequest request)
            => View(new EventFilterViewModel(request, Service.GetAllCities(), CategoryService.GetAllCategories<BaseCategoryProjection>(),
                await _eventUIService.GetEventsListRenderAsync(request)));

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult FileTest()
        {
            return View();
        }

    }
}
