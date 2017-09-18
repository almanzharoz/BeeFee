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

		public IActionResult Index(string id)
		{
			ViewBag.CompanyId = id;
			return View(Service.GetMyEvents(id));
		}

		[HttpGet]
	    public IActionResult Add(string companyId)
			=> View(new AddEventEditModel(companyId, CategoryService.GetAllCategories<BaseCategoryProjection>())
			{
				StartDateTime = DateTime.Now,
				FinishDateTime = DateTime.Now.AddDays(1)
			});

	    [HttpPost]
	    public IActionResult Add(AddEventEditModel model)
	    {
		    if (ModelState.IsValid)
		    {
			    Service.AddEvent(model.CompanyId,
				    model.CategoryId,
				    model.Name,
					model.Label,
					model.Url,
					model.Cover,
				    model.Type,
				    new EventDateTime(model.StartDateTime, model.FinishDateTime),
				    new Address(model.City, model.Address),
				    //new[] {new TicketPrice {Price = new Price(model.Price)}},
					null,
				    model.Html);
			    return RedirectToAction("Index", new { model.CompanyId });
		    }
		    return View(model.Init(CategoryService.GetAllCategories<BaseCategoryProjection>()));
	    }

		[HttpGet]
		public IActionResult Edit(string id, string companyId)
		{
			var @event = Service.GetEvent(id, companyId);
			if (@event == null)
				return NotFound();
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
				 new EventDateTime(model.StartDateTime, model.FinishDateTime),
				 new Address(model.City, model.Address),
				 model.Type,
				 model.CategoryId,
				 //new[] { new TicketPrice() { Price = new Price(model.Price) } },
				 null,
				 model.Html);
				return RedirectToAction("Index", new { model.CompanyId });
			}
			model.Init(CategoryService.GetAllCategories<BaseCategoryProjection>());
			return View(model);
		}

		public IActionResult Remove(string id, string companyId, int version)
	    {
			Service.RemoveEvent(id, companyId, version);
		    return RedirectToAction("Index", new { companyId });
	    }
    }
}