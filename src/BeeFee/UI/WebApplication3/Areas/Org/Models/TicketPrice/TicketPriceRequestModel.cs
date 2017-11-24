using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Org.Models.TicketPrice
{
	public class TicketPriceRequestModel : IRequestModel, IRequestModelWithId, IRequestModelWithParent, IRequestModelWithVersion
	{
		public string Tid { get; set; }
		public string Id { get; set; }
		public string ParentId { get; set; }
		public int Version { get; set; }
	}
}