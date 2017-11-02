﻿using System;
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
		private readonly string _registratorHost;

		public HomeController(ImageService service, ImagesWebServerSettings settings)
	    {
		    _service = service;
			_registratorHost = settings.RegistratorHost;
		}

		[HttpPost]
		[RequestSizeLimit(10000000)]
		public async Task<JsonResult> Post(Model model)
			=> Json(await model.ConsoleLog(x => $"POST: Host: \"{Request.HttpContext.Connection.RemoteIpAddress.ToString()}\", Company: \"{x.CompanyName}\", Event: \"{x.EventName}\", File: \"{x.Filename ?? x.File.FileName}\", Setting: \"{x.Setting}\"").File.OpenReadStream()
				.Using(stream => String.IsNullOrWhiteSpace(model.EventName)
					? _service.AddCompanyLogo(stream, model.CompanyName, GetHost())
					: _service.AddEventImage(stream, model.CompanyName, model.EventName, model.Filename ?? model.File.FileName,
						model.Setting, GetHost())));

		[HttpGet("{companyName}")]
		public void Get(string companyName, string host)
		{
			Console.WriteLine($"Host: \"{Request.HttpContext.Connection.RemoteIpAddress.ToString()}\". Get access for host: \"{host}\", Company: \"{companyName}\"");
			if (_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			_service.GetAccessToFolder(host, companyName, false);
		}

		[HttpGet("{companyName}/{eventName}")]
		public void Get(string companyName, string eventName, string host)
		{
			Console.WriteLine($"Host: \"{Request.HttpContext.Connection.RemoteIpAddress.ToString()}\". Get access for host: \"{host}\", Company: \"{companyName}\", Event: \"{eventName}\"");
			if (_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			_service.GetAccessToFolder(host, companyName, eventName);
		}

		[HttpPut("{companyName}/{eventName}")]
        public bool Put(string companyName, string eventName, string host)
		{
			if (_registratorHost != Request.HttpContext.Connection.RemoteIpAddress.ToString())
				throw new AccessDeniedException();
			_service.GetAccessToFolder(host, companyName, eventName);
			return _service.RegisterEvent(companyName, eventName, "server");
		}

        [HttpDelete("{companyName}/{eventName}")]
        public void Delete(string companyName, string eventName, string filename)
			=> _service.RemoveEventImage(companyName, eventName, filename, GetHost());

		private string GetHost()
			=> _registratorHost == Request.HttpContext.Connection.RemoteIpAddress.ToString() ? "server" : Request.HttpContext.Connection.RemoteIpAddress.ToString();
	}
}
