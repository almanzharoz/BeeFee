using WebApplication3.Models.Interfaces;

namespace WebApplication3.Models
{
    public class EventRequestModel : IRequestModel, IRequestModelWithId, IRequestModelWithParent
    {
		public string Id { get; set; }
		public string ParentId { get; set; }
	}
}
