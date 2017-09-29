using System.ComponentModel.DataAnnotations;
using BeeFee.OrganizerApp.Projections.Company;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class CompanyEditModel
	{
		[Required(ErrorMessage = "Id is required")]
		public string Id { get; set; }
		[Required(ErrorMessage = "Version is required")]
		public int Version { get; set; }

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
		[RegularExpression(@"[a-zA-Z-_]{3,}")]
		public string Url { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public string Logo { get; set; }

		public CompanyEditModel() { }

		public CompanyEditModel(CompanyProjection company)
		{
			Id = company.Id;
			Version = company.Version;
			Name = company.Name;
			Url = company.Url;
			Email = company.Email;
			Logo = company.Logo;
		}
	}
}