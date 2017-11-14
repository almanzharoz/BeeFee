using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.Account
{
	public class LoginModel
	{
		public string ReturnUrl { get; set; }
		[Required]
		public string Login { get; set; }
		[Required]
		public string Pass { get; set; }
	}
}