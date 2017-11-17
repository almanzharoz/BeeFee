using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models.Events;

namespace WebApplication3.Controllers
{
	public class EventsController : BaseController<EventService>
	{
		private readonly CategoryService _categoryService;

		public EventsController(EventService service, CategoryService categoryService) : base(service)
		{
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index(EventsFilter filterModel)
			=> View(filterModel.Load(await Service.SearchEvents())
				.Init(Service.GetAllCities(), _categoryService.GetAllCategories<BaseCategoryProjection>()));

	}
}