using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp2.Embed;
using BeeFee.ImageApp2.Exceptions;
using BeeFee.ImageApp2.Services;
using BeeFee.ImagesWebApplication2.Models;
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

	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		private readonly ImageService _service;
		protected string RequestHost => Request.HttpContext.Connection.RemoteIpAddress.ToString();

		public HomeController(ImageService service)
		{
			_service = service;
		}

		//[RequestSizeLimit(5000000)]
		//public async Task<JsonResult> Post(PostModel model)
		//{
		//	return Json(await _service.AddAsync(model.Directory, RequestHost, model.Token,
		//		(model.File ?? Request.Form.Files[0]).OpenReadStream(), model.Filename ?? (model.File ?? Request.Form.Files[0]).FileName));
		//}

		[HttpPost]
		[RequestSizeLimit(5000000)]
		public JsonResult Post(PostModel model)
		{
			return Json(_service.Add(model.Directory, RequestHost, model.Token,
				(model.File ?? Request.Form.Files[0]).OpenReadStream(), model.Filename ?? (model.File ?? Request.Form.Files[0]).FileName));
		}

		[HttpGet("access")]
		public bool Get(string remoteIp, string directory, string token)
			=> _service.GetAccess(directory, remoteIp, token, RequestHost);

		[HttpPut]
		public bool Put(AcceptModel acceptModel)
			=> _service.AcceptFile(new []{acceptModel.CreateImageSettings()}, RequestHost);
		

		[HttpGet("list")]
		public JsonResult Get(string directory, string token)
			 => Json(_service.GetListOfFiles(directory, RequestHost, token));
	}
}