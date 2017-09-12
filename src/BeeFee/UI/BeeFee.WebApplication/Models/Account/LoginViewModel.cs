using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Models.Account
{
	public class LoginViewModel
	{
		public string ReturnUrl { get; set; }
		[Required]
		public string Login { get; set; }
		[Required]
		public string Pass { get; set; }
	}
}