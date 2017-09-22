using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.TestsApp.Projections
{
	internal class NewEventTransaction : BaseNewEntity, IProjection<EventTransaction>
	{
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }

		public TicketPrice[] Prices { get; }

		public int TicketsLeft { get; }

		public NewEventTransaction(NewEvent @event)
		{
			Event = new EventJoinProjection(@event.Id);
			Company = @event.Parent;
			Prices = @event.Prices;
			TicketsLeft = @event.Prices.Sum(x => x.Left);
		}
	}

	public class EventJoinProjection : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Event>, IJoinProjection, IGetProjection
	{
		public string Url { get; private set; }
		public TicketPrice[] Prices { get; private set; }
		public EventJoinProjection(string id) : base(id)
		{
		}
	}

}