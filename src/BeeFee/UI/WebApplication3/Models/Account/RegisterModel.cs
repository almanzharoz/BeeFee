using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.Account
{
	public class RegisterModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password")]
		public string RetryPassword { get; set; }
		[EmailAddress]
		[Required]
		public string Email { get; set; }
	}
}