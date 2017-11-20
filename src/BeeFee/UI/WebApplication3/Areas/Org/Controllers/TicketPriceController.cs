using System.Threading.Tasks;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Org.Models.TicketPrice;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Org.Controllers
{
	public class TicketPriceController : BaseController<EventService, TicketPriceRequestModel>
	{
		public TicketPriceController(EventService service, TicketPriceRequestModel model) : base(service, model)
		{
		}

		[HttpGet]
		public Task<IActionResult> Edit()
			=> View("EditTicket",
				m => Service.GetEventTicketAsync(m.Id, m.ParentId, m.Tid),
				c => new TicketPriceEditModel(c));

		[HttpPost]
		public Task<IActionResult> Edit(TicketPriceEditModel model)
			=> ModelStateIsValid(model,
				m => Service.UpdateEventTicketPriceAsync(Model.Id, Model.ParentId, Model.Tid, m.Name, m.Description, m.Price, m.Count, m.Template),
				m => RedirectToActionPermanent("Prices", "Event", new { area = "Org", Model.Id, Model.ParentId }));

		public async Task<IActionResult> Remove()
		{
			await Service.RemoveTicketPriceAsync(Model.Id, Model.ParentId, Model.Tid);
			return RedirectToActionPermanent("", "Event", new {area="Org", Model.Id, Model.ParentId});
		}
	}
}