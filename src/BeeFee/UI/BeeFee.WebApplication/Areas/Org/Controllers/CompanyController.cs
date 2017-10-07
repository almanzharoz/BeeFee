﻿using BeeFee.Model.Embed;
using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Services;
using BeeFee.WebApplication.Areas.Org.Models;
using BeeFee.WebApplication.Controllers;
using BeeFee.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.Organizer)]
	public class CompanyController : BaseController<CompanyService>
	{
        private readonly ImagesService _imagesService;

		public CompanyController(CompanyService service, CategoryService categoryService) : base(service, categoryService)
		{
            _imagesService = new ImagesService(BeeFeeWebAppSettings.Instance.ImagesUrl);
		}

		public IActionResult Index()
			=> View(Service.GetMyCompanies());

		[HttpGet]
		public IActionResult Add()
			=> View(new AddCompanyEditModel());

		[HttpPost]
		public IActionResult Add(AddCompanyEditModel model)
			=> ModelState.IsValid.If(
				() => Service.AddCompany(model.Name, model.Url)
					.IfNotNull<CompanyProjection, IActionResult>(x =>
					{
						if (model.File != null && model.File.Length > 0)
							_imagesService.AddCompanyLogo(x.Url, model.File.OpenReadStream());
						return RedirectToActionPermanent("Index");
					}, () => View("SaveError")),
				() => View(model));

		[HttpGet]
		public IActionResult Edit(string id)
			=> View(new CompanyEditModel(Service.GetCompany(id).Fluent(x => _imagesService.GetAccessToFolder(x.Url, Request.Host.Host))));

		[HttpPost]
		public IActionResult Edit(CompanyEditModel model)
			=> ModelState.IsValid.If(
				() => Service.EditCompany(model.Id, model.Version, model.Name, model.Url, model.Email, model.Logo)
					.If<IActionResult>(() => RedirectToActionPermanent("Index"), () => View("SaveError")),
				() => View(model));

		public IActionResult Remove(string id, int version)
		{
			Service.RemoveCompany(id, version);
			return RedirectToActionPermanent("Index");
		}
	}
}