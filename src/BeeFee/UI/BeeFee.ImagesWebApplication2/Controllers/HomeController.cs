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
		public string Token { get; set; }
	}

	public class AcceptModel
	{
		public List<ImageSettings> Images { get; set; }
	}

	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		private readonly ImageService _service;
		protected string RequestHost => Request.HttpContext.Connection.RemoteIpAddress.ToString();

		public HomeController(ImageService service)
		{
			_service = service;
		}

		[RequestSizeLimit(5000000)]
		public async Task<JsonResult> Post(PostModel model)
		{
			return Json(await _service.Add(model.Directory, RequestHost, model.Token,
				(model.File ?? Request.Form.Files[0]).OpenReadStream(), model.Filename ?? (model.File ?? Request.Form.Files[0]).FileName));
		}

		[HttpGet("access")]
		public bool Get(string remoteIp, string directory, string token)
			=> _service.GetAccess(directory, remoteIp, token, RequestHost);

		public Task<bool> Put(AcceptModel acceptModel)
			=> _service.AcceptFiles(acceptModel.Images, RequestHost);
		

		[HttpGet("list")]
		public JsonResult Get(string directory, string token)
			 => Json(_service.GetListOfFiles(directory, RequestHost, token));
	}
}