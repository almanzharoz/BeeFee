using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Areas.Org.Models.Companies;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = RoleNames.User)]
	public class CompaniesController : BaseController<CompanyService>
	{
		private readonly ImagesService _imagesService;

		public CompaniesController(CompanyService service, ImagesService imagesService) : base(service)
		{
			_imagesService = imagesService;
		}

		#region List
		[Authorize(Roles = RoleNames.MultiOrganizer)]
		public async Task<IActionResult> Index(CompaniesFilter model)
			=> View(model.Load(await Service.GetMyCompaniesAsync()));
		#endregion

		public ActionResult First() //hack
			=> RedirectToActionPermanent("Edit", "Company", new {area = "Org", id = Service.GetCompany(null).Id});
		public ActionResult FirstEvents() //hack
			=> RedirectToActionPermanent("Events", "Company", new { area = "Org", id = Service.GetCompany(null).Id });

		#region Create
		[HttpGet]
		public ActionResult Create()
			=> View(new CreateCompanyModel());

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public Task<IActionResult> Create(CreateCompanyModel model)
			=> ModelStateIsValid(model,
				m => Service.AddCompanyAsync(m.Name, m.Url, m.Email,
					"company.jpg" /*m.File != null && m.File.Length > 0 ? m.File.FileName : null*/),
				async (m, c) =>
				{
					if (m.File != null && m.File.Length > 0)
						await _imagesService.AddCompanyLogo(c.Url, m.File.OpenReadStream());

					if (await Service.StartOrgAsync())
						return RedirectToActionPermanent("Relogin", "Account", new {area = "", returnUrl = "/Org/Company/Create/" + c.Id});
					return RedirectToActionPermanent("Index");
				});

		#endregion

	}
}