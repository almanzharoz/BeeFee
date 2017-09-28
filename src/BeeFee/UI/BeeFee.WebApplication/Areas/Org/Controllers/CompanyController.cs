using BeeFee.Model.Services;
using BeeFee.OrganizerApp.Services;
using BeeFee.WebApplication.Areas.Org.Models;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = "organizer")]
	public class CompanyController : BaseController<CompanyService>
	{
		public CompanyController(CompanyService service, CategoryService categoryService) : base(service, categoryService)
		{
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
					.If<IActionResult>(() => RedirectToActionPermanent("Index"), () => View("SaveError")),
				() => View(model));

		[HttpGet]
		public IActionResult Edit(string id)
			=> View(new CompanyEditModel(Service.GetCompany(id)));

		[HttpPost]
		public IActionResult Edit(CompanyEditModel model)
			=> ModelState.IsValid.If(
				() => Service.EditCompany(model.Id, model.Version, model.Name, model.Url, model.Email)
					.If<IActionResult>(() => RedirectToActionPermanent("Index"), () => View("SaveError")),
				() => View(model));

		public IActionResult Remove(string id, int version)
		{
			Service.RemoveCompany(id, version);
			return RedirectToActionPermanent("Index");
		}
	}
}