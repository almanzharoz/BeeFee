using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class EventIdModel : IIdModel, IParentModel, IModelWithVersion
	{
		public string Id { get; set; }
		public string ParentId { get; set; }
		public int Version { get; set; }
	}
}