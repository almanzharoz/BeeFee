using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Admin.Models
{
	public class UserRequestModel : IRequestModel, IRequestModelWithId
	{
		public string Id { get; set; }
	}
}