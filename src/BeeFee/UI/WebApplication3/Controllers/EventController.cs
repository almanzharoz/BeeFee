﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Infrastructure;
using WebApplication3.Models;
using WebApplication3.Models.Event;

namespace WebApplication3.Controllers
{
    public class EventController : BaseController<EventService, EventRequestModel>
    {
		private readonly ImagesHelper _imagesHelper;

		public EventController(EventService service, ImagesHelper imagesHelper, EventRequestModel model) : base(service, model)
		{
			_imagesHelper = imagesHelper;
		}

		[Route("event/{parentid}/{id}")]
		[HttpGet]
		public Task<IActionResult> Index()
			=> View("Index",
				m => Service.GetEventByUrl(m.ParentId, m.Id),
				async e => new EventPageModel(e,
					await Service.GetEventTransaction(e.Id),
					User.Identity.Name /* TODO: Доставать юзера из БД */,
					User.Claims.Where(c => c.Type.Equals(ClaimTypes.Email, StringComparison.Ordinal)).Select(c => c.Value)
						.FirstOrDefault(), "", Service.CanRegister(e.Id, e.Parent.Id, HttpContext.Session.Fluent(x => x.SetString("1", "1")/*TODO: Hack. Иначе сессия всегда новая будет*/)?.Id) ? null : (bool?)false));

		#region Register

		[HttpPost]
		public Task<IActionResult> Register(EventPageModel model)
			=> ModelStateIsValid(model, async m =>
					await (await Service.GetEventByUrl(Model.ParentId, Model.Id)).Convert(e =>
						Service.RegisterToEventAsync(e.Id, e.Parent.Id, m.Email, m.Name, m.Phone, m.TicketId,
							_imagesHelper.GetImageUrl("/event", e.Page.Cover), HttpContext.Session?.Id)),
				m => View("Index", vm => Service.GetEventByUrl(vm.ParentId, vm.Id),
					async e => new EventPageModel(e, await Service.GetEventTransaction(e.Id), true)),
				m => View("Index", vm => Service.GetEventByUrl(vm.ParentId, vm.Id),
					async e => new EventPageModel(e, await Service.GetEventTransaction(e.Id), false)));

		#endregion

		#region Contact
		[HttpGet]
		public ActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Contact(object model)
		{
			return View();
		}
		#endregion

	}
}