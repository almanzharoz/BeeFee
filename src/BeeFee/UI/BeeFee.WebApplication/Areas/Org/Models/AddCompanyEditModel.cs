using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class AddCompanyEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		public string Url { get; set; }

	}
}