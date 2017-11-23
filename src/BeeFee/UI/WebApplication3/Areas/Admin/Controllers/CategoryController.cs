using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication3.Areas.Admin.Models;
using WebApplication3.Areas.Admin.Models.Category;
using WebApplication3.Controllers;

namespace WebApplication3.Areas.Admin.Controllers
{
	[Area("Moderator")]
	[Authorize(Roles = RoleNames.Admin)]
	public class CategoryController : BaseController<CategoryService, CategoryRequestModel>
	{
		public CategoryController(CategoryService service, CategoryRequestModel model) : base(service, model)
		{
		}

		[HttpGet]
		public Task<IActionResult> Edit()
			=> View("Edit",
				m => Service.GetCategory(m.Id, m.Version),
				p => new CategoryEditModel(p));

		[HttpPost]
		public Task<IActionResult> Edit(CategoryEditModel model)
			=> ModelStateIsValid(model, 
				m => Service.Rename(Model.Id, Model.Version, m.Name, m.Url),
				m => RedirectToActionPermanent("Index", "Categories"),
				View);

		public async Task<IActionResult> Remove()
		{
			await Service.Remove(Model.Id, Model.Version);
			return RedirectToActionPermanent("Index", "Categories");
		}
	}
}