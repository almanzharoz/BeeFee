using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Models.Account
{
    public class SetPasswordEditModel
    {
		[Required]
		public string VerifyEmail { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password")]
		public string RetryPassword { get; set; }

		public SetPasswordEditModel() { }

		public SetPasswordEditModel(string verifyEmail)
		{
			VerifyEmail = verifyEmail;
		}
	}
}
