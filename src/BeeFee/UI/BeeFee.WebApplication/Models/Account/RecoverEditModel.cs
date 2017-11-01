using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Models.Account
{
    public class RecoverEditModel
    {
		[Required]
		[EmailAddress]
		public string Email { get; set; }
    }
}
