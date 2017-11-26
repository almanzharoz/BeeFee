using System.Threading.Tasks;
using BeeFee.ImageApp2.Exceptions;
using BeeFee.ImageApp2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.ImagesWebApplication2.Controllers
{
	public class Model
	{
		public IFormFile File { get; set; }
		public string Filename { get; set; }
		public string Directory { get; set; }
	}

	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		private readonly ImageService _service;
		private readonly string _registratorHost;

		public HomeController(ImageService service, ImagesWebServerSettings settings)
		{
			_service = service;
			_registratorHost = settings.RegistratorHost;
		}

		[HttpPost]
		[RequestSizeLimit(5000000)]
		public async Task<JsonResult> Post(Model model)
		{
			return Json(await _service.Add(model.Directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), "",
				(model.File ?? Request.Form.Files[0]).OpenReadStream(), model.Filename ?? (model.File ?? Request.Form.Files[0]).FileName));
		}

		[HttpGet("getaccess/{directory}/{remoteIp}/token}")]
		public void Get(string remoteIp, string directory, string token, string host)
		{
			if(_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			_service.GetAccess(directory, remoteIp, token, host);
		}

		[HttpGet("accept/")] //TODO
		

		[HttpGet("getlist/{directory}")]
		public JsonResult Get(string directory)
		{
			return Json(_service.GetListOfFiles(directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), ""));
		}

		[HttpGet("rename/{directory}/{oldFileName}/{newFileName}")]
		public void Get(string directory, string oldFileName, string newFileName)
		{
			_service.Rename(directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), "", oldFileName, newFileName);
		}

		[HttpGet("remove/{directory}/{fileName}")]
		public void Get(string directory, string fileName)
		{
			_service.Remove(directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), "", fileName);
		}
	}
}