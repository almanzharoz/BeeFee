using System;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Services;
using BeeFee.WebApplication.Areas.Org.Models;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = "organizer")]
	public class EventController : BaseController<EventService>
	{
		public EventController(EventService service, CategoryService categoryService) : base(service, categoryService)
		{
		}

		public IActionResult Index()
	    {
		    return View(_service.GetMyEvents());
	    }

		[HttpGet]
	    public IActionResult Add()
		{
			return View(new AddEventEditModel(_categoryService.GetAllCategories<CategoryProjection>())
			{
				StartDateTime = DateTime.Now,
				FinishDateTime = DateTime.Now.AddDays(1)
			});
		}

	    [HttpPost]
	    public IActionResult Add(AddEventEditModel model)
	    {
		    if (ModelState.IsValid)
		    {
			    _service.AddEvent(
				    model.CategoryId,
				    model.Name,
					model.Url,
					model.Cover,
				    model.Type,
				    new EventDateTime(model.StartDateTime, model.FinishDateTime),
				    new Address(model.City, model.Address),
				    //new[] {new TicketPrice {Price = new Price(model.Price)}},
					null,
				    model.Html);
			    return RedirectToAction("Index");
		    }
		    return View(model.Init(_categoryService.GetAllCategories<CategoryProjection>()));
	    }

		[HttpGet]
		public IActionResult Edit(string id)
		{
			var @event = _service.GetEvent(id);
			if (@event == null)
				return NotFound();
			return View(new EventEditModel(@event, _categoryService.GetAllCategories<CategoryProjection>()));
		}

		[HttpPost]
		public IActionResult Edit(EventEditModel model)
		{
			if (ModelState.IsValid)
			{
				_service.UpdateEvent(
				 model.Id,
				 model.Version,
				 model.Name,
				 model.Url,
				 model.Cover,
				 new EventDateTime(model.StartDateTime, model.FinishDateTime),
				 new Address(model.City, model.Address),
				 model.Type,
				 model.CategoryId,
				 //new[] { new TicketPrice() { Price = new Price(model.Price) } },
				 null,
				 model.Html);
				return RedirectToAction("Index");
			}
			model.Init(_categoryService.GetAllCategories<CategoryProjection>());
			return View(model);
		}

		public IActionResult Remove(string id, int version)
	    {
			_service.RemoveEvent(id, version);
		    return RedirectToAction("Index");
	    }
    }
}