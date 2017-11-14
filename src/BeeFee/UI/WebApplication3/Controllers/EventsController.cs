using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index(object filterModel)
        {
            return View();
        }
    }
}