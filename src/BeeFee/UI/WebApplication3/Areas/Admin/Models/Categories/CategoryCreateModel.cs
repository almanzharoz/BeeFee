using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Areas.Admin.Models.Categories
{
	public class CategoryCreateModel
	{
		[Required]
		public string Name { get; set; }
		public string Url { get; set; }
	}
}