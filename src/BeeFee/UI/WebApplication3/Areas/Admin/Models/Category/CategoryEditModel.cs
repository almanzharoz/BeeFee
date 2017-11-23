using System.ComponentModel.DataAnnotations;
using BeeFee.AdminApp.Projections.Category;

namespace WebApplication3.Areas.Admin.Models.Category
{
	public class CategoryEditModel
	{
		[Required]
		public string Name { get; set; }
		public string Url { get; set; }

		public CategoryEditModel() { }

		public CategoryEditModel(CategoryProjection category)
		{
			Name = category.Name;
			Url = category.Url;
		}
	}
}