using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.Account
{
	public class SetPasswordModel
	{
		[Required]
		public string VerifyEmail { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password")]
		public string RetryPassword { get; set; }

		public SetPasswordModel() { }

		public SetPasswordModel(string verifyEmail)
		{
			VerifyEmail = verifyEmail;
		}
	}
}