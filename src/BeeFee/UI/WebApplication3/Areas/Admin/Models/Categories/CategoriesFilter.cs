using System.Collections.Generic;
using BeeFee.AdminApp.Projections.Category;

namespace WebApplication3.Areas.Admin.Models.Categories
{
	public class CategoriesFilter
	{
		public IReadOnlyCollection<CategoryProjection> Items { get; private set; }

		public CategoriesFilter Load(IReadOnlyCollection<CategoryProjection> items)
		{
			Items = items;
			return this;
		}
	}
}