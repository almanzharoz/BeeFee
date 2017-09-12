using BeeFee.AdminApp.Services;
using BeeFee.WebApplication.Areas.Admin.Models;
using BeeFee.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
    public class CategoryController : BaseController<CategoryService>
    {
	    public CategoryController(CategoryService service, Model.Services.CategoryService categoryService) : base(service, categoryService)
	    {
	    }

		public IActionResult Index()
        {
            return View(new AddCategoryListModel(_service.GetAllCategories()));
        }

	    [HttpPost]
	    public IActionResult Add(AddCategoryListModel addCategoryListModel)
	    {
		    _service.Add(addCategoryListModel.Name, addCategoryListModel.Url);
		    return RedirectToAction("Index");
	    }

	    public IActionResult Remove(string id)
	    {
		    _service.Remove(id);
		    return RedirectToAction("Index");
	    }
    }
}