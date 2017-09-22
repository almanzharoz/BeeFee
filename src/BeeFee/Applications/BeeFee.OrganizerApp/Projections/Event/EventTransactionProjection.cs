using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Event
{
    internal class EventTransactionProjection : BaseEntity, IProjection<EventTransaction>, ISearchProjection, IRemoveProjection
    {
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }

		public TicketPrice[] Prices { get; }

		public int TicketsLeft { get; }

		public Transaction[] Transactions { get; }

		public EventTransactionProjection(string id, BaseCompanyProjection company, EventJoinProjection @event, TicketPrice[] prices, int ticketsLeft, Transaction[] transactions) : base(id)
		{
			Company = company;
			Event = @event;
			Prices = prices;
			TicketsLeft = ticketsLeft;
			Transactions = transactions;
		}
	}
}
