using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Infrastructure;
using BeeFee.WebApplication.Infrastructure.Services;
using BeeFee.WebApplication.Models;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Controllers
{
    public class HomeController : BaseController<EventService>
    {
        private readonly EventUIService _eventUIService;
        private readonly ExceptionService _exceptionService;
        public HomeController(EventService service, CategoryService categoryService, EventUIService eventUiService, ExceptionService exceptionService) : base(service, categoryService)
        {
            _eventUIService = eventUiService;
            _exceptionService = exceptionService;
        }

        public async Task<IActionResult> Index(LoadEventsRequest request)
            => View(new EventFilterViewModel(request, Service.GetAllCities(), CategoryService.GetAllCategories<BaseCategoryProjection>(),
                await _eventUIService.GetEventsListRenderAsync(request)));

        public IActionResult Error()
        {
            return View(new ErrorPageModel(""));
        }

        public IActionResult Exception()
        {
           throw new ArgumentNullException("yep");
        }

        public IActionResult FileTest()
        {
            return View();
        }

    }
}
