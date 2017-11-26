using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.ImageApp2.Embed;
using BeeFee.ImageApp2.Exceptions;
using BeeFee.ImageApp2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.ImagesWebApplication2.Controllers
{
	public class PostModel
	{
		public IFormFile File { get; set; }
		public string Filename { get; set; }
		public string Directory { get; set; }
	}

	public class AcceptModel
	{
		public List<ImageSettings> Images { get; set; }
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
		public async Task<JsonResult> Post(PostModel postModel)
		{
			return Json(await _service.Add(postModel.Directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), "",
				(postModel.File ?? Request.Form.Files[0]).OpenReadStream(), postModel.Filename ?? (postModel.File ?? Request.Form.Files[0]).FileName));
		}

		[HttpGet("getaccess/{directory}/{remoteIp}/token}")]
		public void Get(string remoteIp, string directory, string token)
		{
			if(_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			_service.GetAccess(directory, remoteIp, token, Request.HttpContext.Connection.RemoteIpAddress.ToString());
		}

		[HttpPost("accept")] //TODO
		public async Task Accept(AcceptModel acceptModel)
		{
			if(_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			await _service.AcceptFiles(acceptModel.Images, Request.HttpContext.Connection.RemoteIpAddress.ToString());
		}
		

		[HttpGet("getlist/{directory}")]
		public JsonResult Get(string directory)
		{
			return Json(_service.GetListOfFiles(directory, Request.HttpContext.Connection.RemoteIpAddress.ToString(), ""));
		}
	}
}