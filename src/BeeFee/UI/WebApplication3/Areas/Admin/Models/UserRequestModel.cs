using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Admin.Models
{
	public class UserRequestModel : IRequestModelWithId
	{
		public string Id { get; set; }
	}
}