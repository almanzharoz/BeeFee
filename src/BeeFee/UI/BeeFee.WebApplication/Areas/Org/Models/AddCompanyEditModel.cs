using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class AddCompanyEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[RegularExpression(@"[a-zA-Z-_\d]{1,}")]
		public string Url { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		public IFormFile File { get; set; }
	}
}