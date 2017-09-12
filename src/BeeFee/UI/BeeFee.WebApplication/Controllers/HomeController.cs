using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Controllers
{
    public class HomeController : BaseController<EventService>
    {
        public HomeController(EventService service, CategoryService categoryService) : base(service, categoryService)
        {
        }

        public IActionResult Index(LoadEventsRequest request)
            => View(new EventFilterViewModel(request, _service.GetAllCities(), _categoryService.GetAllCategories<CategoryProjection>(), 
				_service.SearchEvents(request.Text, request.City, request.Categories, request.Types, request.StartDate, request.EndDate, request.MaxPrice, 9, request.PageIndex)));

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
