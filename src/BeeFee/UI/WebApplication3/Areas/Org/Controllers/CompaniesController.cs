using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
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
			=> View(await Service.GetMyCompaniesAsync(model.Page, model.Size));
		#endregion

		#region Create
		[HttpGet]
		public ActionResult Create()
			=> View(new CreateCompanyModel());

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public Task<IActionResult> Create(CreateCompanyModel model)
			=> ModelStateIsValid(model,
				m => Service.AddCompanyAsync(m.Name, m.Url, m.Email,
						"company.jpg" /*m.File != null && m.File.Length > 0 ? m.File.FileName : null*/)
					.IfNotNullAsync<CompanyProjection, IActionResult>(async x =>
					{
						if (m.File != null && m.File.Length > 0)
							await _imagesService.AddCompanyLogo(x.Url, m.File.OpenReadStream());
						if (Service.StartOrg())
							return RedirectToActionPermanent("Relogin", "Account",
								new {area = "", returnUrl = "/Org/Company/Create/" + x.Id});
						return RedirectToActionPermanent("Index");
					}, () => Task.FromResult((IActionResult) View("SaveError"))),
				m => Task.FromResult((IActionResult) View(m)));

		#endregion

	}
}