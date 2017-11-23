using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication3.Areas.Admin.Models.Categories;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
	[Area("Moderator")]
	[Authorize(Roles = RoleNames.Admin)]
	public class CategoriesController : BaseController<CategoryService>
    {
		public CategoriesController(CategoryService service) : base(service)
		{
		}

		public IActionResult Index(CategoriesFilter model)
			=> View(model.Load(Service.GetAllCategories()));

		[HttpGet]
		public IActionResult Create()
			=> View(new CategoryCreateModel());

		[HttpPost]
		public Task<IActionResult> Create(CategoryCreateModel model)
			=> ModelStateIsValid(model,
				m => Service.Add(m.Name, m.Url),
				m => RedirectToActionPermanent("Index"),
				View);
	}
}