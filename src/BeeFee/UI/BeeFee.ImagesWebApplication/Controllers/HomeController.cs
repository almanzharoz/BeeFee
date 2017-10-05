using System.Threading.Tasks;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.ImagesWebApplication.Controllers
{
	public class Model
	{
		public string Setting { get; set; }
		public IFormFile File { get; set; }
		public string Filename { get; set; }
		public string CompanyName { get; set; }
		public string EventName { get; set; }
	}

    [Route("api/[controller]")]
    public class HomeController : Controller
    {
	    private readonly ImageService _service;
		private readonly string _registratorHost = "localhost";


		public HomeController(ImageService service)
	    {
		    _service = service;
	    }

		[HttpPost]
		[RequestSizeLimit(10000000)]
		public async Task<JsonResult> Post(Model model)
			=> Json(await model.File.OpenReadStream()
				.Using(stream =>
					_service.AddEventImage(stream, model.CompanyName, model.EventName, model.Filename ?? model.File.FileName,
						model.Setting, Request.Host.Host)));

		[HttpGet("{companyName}")]
		public void Get(string companyName, string host)
		{
			if (_registratorHost != Request.Host.Host)
				throw new AccessDeniedException();
			_service.GetAccessToFolder(host, companyName);
		}

		[HttpPut("{companyName}/{eventName}")]
        public bool Put(string companyName, string eventName, string host)
		{
			if (_registratorHost != Request.Host.Host)
				throw new AccessDeniedException();
			return _service.RegisterEvent(companyName, eventName, host);
		}

        [HttpDelete("{companyName}/{eventName}")]
        public void Delete(string companyName, string eventName, string filename)
			=> _service.RemoveEventImage(companyName, eventName, filename, Request.Host.Host);
		
	}
}
