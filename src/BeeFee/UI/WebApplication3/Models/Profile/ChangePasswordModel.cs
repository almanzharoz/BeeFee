using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.Profile
{
	public class ChangePasswordModel
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