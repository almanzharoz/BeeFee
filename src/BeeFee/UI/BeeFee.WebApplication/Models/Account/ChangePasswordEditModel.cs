using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Models.Account
{
	public class ChangePasswordEditModel
	{
		[Required]
		public string OldPassword { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password")]
		public string RetryPassword { get; set; }

	}
}