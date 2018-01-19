using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Antiforgery;
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
		private readonly IAntiforgery _antiforgery;

		public CompaniesController(CompanyService service, ImagesService imagesService, IAntiforgery antiforgery) : base(service)
		{
			_imagesService = imagesService;
			_antiforgery = antiforgery;
		}

		#region List
		[Authorize(Roles = RoleNames.MultiOrganizer)]
		public async Task<IActionResult> Index(CompaniesFilter model)
			=> View(model.Load(await Service.GetMyCompaniesAsync()));
		#endregion

		public ActionResult First() //hack
			=> Service.GetCompany(null).Convert(c => RedirectToAction("Edit", "Company", new {area = "Org", id = c.Id, version= c.Version}));
		public ActionResult FirstEvents() //hack
			=> RedirectToAction("Events", "Company", new { area = "Org", id = Service.GetCompany(null).Id });

		#region Create

		[HttpGet]
		public ActionResult Create()
		{
			_imagesService.GetAccess("/logo", _antiforgery.GetAndStoreTokens(HttpContext).RequestToken, UserHost);
			return View(new CreateCompanyModel());
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public Task<IActionResult> Create(CreateCompanyModel model)
			=> ModelStateIsValid(model,
				async m =>
				{
					await _imagesService.Accept(new AcceptModel
					{
						TempPath = model.File,
						Images = new[] {new ImageSaveSetting("/logo", 200, 200)}
					});
					return await Service.AddCompanyAsync(m.Name, m.Url, m.Email, model.File);
				},
				async (m, c) =>
				{
					if (await Service.StartOrgAsync())
						return RedirectToAction("Relogin", "Account", new {area = "", returnUrl = "/Org/Company/CreateEvent/" + c.Id});
					return RedirectToAction("Index");
				});

		#endregion

	}
}