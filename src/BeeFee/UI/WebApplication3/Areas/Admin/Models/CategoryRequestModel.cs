using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Admin.Models
{
    public class CategoryRequestModel : IRequestModel, IRequestModelWithId, IRequestModelWithVersion
	{
		public string Id { get; set; }
		public int Version { get; set; }
	}
}
