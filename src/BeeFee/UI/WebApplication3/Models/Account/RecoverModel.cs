using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.Account
{
	public class RecoverModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}