using System.ComponentModel.DataAnnotations;
using BeeFee.Model.Embed;

namespace BeeFee.WebApplication.Areas.Admin.Models
{
	public class AddUserModel
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public EUserRole[] Roles { get; set; }
	}
}