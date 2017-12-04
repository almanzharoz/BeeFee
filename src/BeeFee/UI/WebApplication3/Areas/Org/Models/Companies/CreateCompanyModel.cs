using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Areas.Org.Models.Companies
{
	public class CreateCompanyModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		// TODO: Добавить клиентскую Remote-проверку
		[RegularExpression(@"[a-zA-Z-_\d]{1,}", ErrorMessage = "Доступны только латинские буквы, цифры и символы \"_\", \"-\"")]
		public string Url { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		public string File { get; set; }
	}
}