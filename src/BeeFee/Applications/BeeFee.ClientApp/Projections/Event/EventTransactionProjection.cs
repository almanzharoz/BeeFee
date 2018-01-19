using System;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventTransactionPricesProjection : BaseEntity, IProjection<EventTransaction>, ISearchProjection
	{
		public TicketPrice[] Prices { get; }

		public int TicketsLeft { get; }

		public EventTransactionPricesProjection(string id, TicketPrice[] prices, int ticketsLeft) : base(id)
		{
			Prices = prices;
			TicketsLeft = ticketsLeft;
		}
	}

	public class EventTransactionProjection : BaseEntity, IProjection<EventTransaction>, IJoinProjection, ISearchProjection, IRemoveProjection
    {
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }

		public TicketPrice[] Prices { get; }

		public int TicketsLeft { get; }

		//public EventTicketTransaction[] Transactions { get; }

		public EventTransactionProjection(string id, BaseCompanyProjection company, EventJoinProjection @event, TicketPrice[] prices, int ticketsLeft/*, EventTicketTransaction[] transactions*/) : base(id)
		{
			Company = company;
			Event = @event;
			Prices = prices;
			TicketsLeft = ticketsLeft;
			//Transactions = transactions;
		}
	}

	public class EventTransactionJoinProjection : BaseEntity, IProjection<EventTransaction>, IJoinProjection
	{
		public EventJoinProjection Event { get; private set; }
		public BaseCompanyProjection Company { get; private set; }

		public TicketPrice[] Prices { get; private set; }

		public int TicketsLeft { get; private set; }

		public EventTransactionJoinProjection(string id) : base(id)
		{
		}
	}

	public class EventTicketTransaction
	{
		public Guid Id { get; }
		public Guid PriceId { get; }
		public DateTime Date { get; }
		public Contact Contact { get; }
		public float Sum { get; }
		public ETransactionType Type { get; }
		public ETransactionState State { get; }
		public EventTransactionJoinProjection Event { get; }

		public EventTicketTransaction(Guid id, Guid priceId, DateTime date, Contact contact, float sum, ETransactionType type, ETransactionState state, EventTransactionJoinProjection @event)
		{
			Id = id;
			PriceId = priceId;
			Date = date;
			Contact = contact;
			Sum = sum;
			Type = type;
			State = state;
			Event = @event;
		}
	}
}
