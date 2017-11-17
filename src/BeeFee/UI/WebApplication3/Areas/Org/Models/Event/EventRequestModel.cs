using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class EventRequestModel : IRequestModelWithId, IRequestModelWithParent, IRequestModelWithVersion
	{
		public string Id { get; set; }
		public string ParentId { get; set; }
		public int Version { get; set; }
	}
}