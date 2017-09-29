using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CategoryService = BeeFee.Model.Services.CategoryService;

namespace BeeFee.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = RoleNames.Admin)]
	public class EventsController : BaseController<EventService>
	{
		public EventsController(EventService service, CategoryService categoryService) : base(service, categoryService)
		{
		}
	}
}