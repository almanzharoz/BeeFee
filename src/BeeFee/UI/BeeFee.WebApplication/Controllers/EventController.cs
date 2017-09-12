using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.Model.Embed;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpFuncExt;

namespace BeeFee.WebApplication.Controllers
{
	public class EventController : BaseController<EventService>
	{
		public EventController(EventService service, CategoryService categoryService) : base(service, categoryService)
		{
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

		public async Task<IActionResult> Event(string id)
			//=> id.IfNotNull(x => _service.GetEventByUrl(id)
			//  .IfNotNull<EventProjection, IActionResult>(View, NotFound),
			// () => Task.FromResult((IActionResult)NotFound())); // ������� ������, ����� �� ���� ����� fluent :)
		{
			var model = await _service.GetEventByUrl(id);
			if (model == null)
				return NotFound();
			return View(model);
		}

		[HttpGet]
		public IActionResult LoadEvents(LoadEventsRequest request)
			=> Json(_service.SearchEvents(request.Text, request.City, request.Categories, request.Types, request.StartDate,
				request.EndDate, request.MaxPrice, request.PageSize, request.PageIndex));

	}
}