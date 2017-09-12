using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BeeFee.AdminApp.Projections.Category;

namespace BeeFee.WebApplication.Areas.Admin.Models
{
	public class AddCategoryListModel
	{
		public IReadOnlyCollection<CategoryProjection> ExistingCategories { get; }
		[Required]
		public string Name { get; set; }
		public string Url { get; set; }

		public AddCategoryListModel()
		{
		}

		public AddCategoryListModel(IReadOnlyCollection<CategoryProjection> existingCategories)
		{
			ExistingCategories = existingCategories;
		}
	}
}