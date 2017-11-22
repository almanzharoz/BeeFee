using System.ComponentModel.DataAnnotations;
using BeeFee.OrganizerApp.Projections.Company;

namespace WebApplication3.Areas.Org.Models.Company
{
	public class CompanyEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		// TODO: Добавить клиентскую Remote-проверку
		[RegularExpression(@"[a-zA-Z-_\d]{1,}", ErrorMessage = "Доступны только латинские буквы, цифры и символы \"_\", \"-\"")]
		public string Url { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public string Logo { get; set; }

		public CompanyEditModel() { }

		public CompanyEditModel(CompanyProjection company)
		{
			Name = company.Name;
			Url = company.Url;
			Email = company.Email;
			Logo = company.Logo;
		}
	}
}