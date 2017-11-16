using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Org.Models.TicketPrice
{
	public class TicketPriceIdModel : IIdModel, IParentModel, IModelWithVersion
	{
		public string Tid { get; set; }
		public string Id { get; set; }
		public string ParentId { get; set; }
		public int Version { get; set; }
	}
}