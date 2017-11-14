using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Areas.Moderator.Controllers
{
    public class EventsController : Controller
    {

		/// <summary>
		/// Список мероприятий, ожидающих публикации
		/// </summary>
        public IActionResult Index(object model)
        {
            return View();
        }
    }
}