using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.JobsApp.Projections
{
	public class EventTicketProjection : BaseEntity, IProjection<EventTransaction>, IGetProjection
	{
		public TicketPrice[] Prices { get; set; }

		public EventTicketProjection(string id, TicketPrice[] prices) : base(id)
		{
			Prices = prices;
		}
	}
}