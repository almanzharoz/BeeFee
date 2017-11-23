using System;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Event
{
	public class EventTransactionPricesProjection : BaseEntity, IProjection<EventTransaction>, ISearchProjection, IUpdateProjection
	{
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }

		public TicketPrice[] Prices { get; private set; }
		public int TicketsLeft { get; private set; }

		public EventTransactionPricesProjection(string id, BaseCompanyProjection company, EventJoinProjection @event, TicketPrice[] prices, int ticketsleft) : base(id)
		{
			Company = company;
			Event = @event;
			Prices = prices;
			TicketsLeft = ticketsleft;
		}

		public EventTransactionPricesProjection UpdatePrice(string tickerPriceId, string name, string description, decimal price, int count, string template)
		{
			var id = Guid.Parse(tickerPriceId);
			var p = Prices.First(x => x.Id == id);
			var left = p.Left;
			TicketsLeft = TicketsLeft + (p.Update(name, description, price, count, template).Left - left);
			return this;
		}

		public EventTransactionPricesProjection RemovePrice(string tickerPriceId)
		{
			var id = Guid.Parse(tickerPriceId);
			var p = Prices.First(x => x.Id == id);
			if (p.Left != p.Count)
				throw new Exception("Нельзя удалить проданные билеты");
			Prices = Prices.Except(new[] {p}).ToArray();
			return this;
		}

		public EventTransactionPricesProjection AddPrice(string name, string description, decimal price, int count, string template)
		{
			Prices = (Prices ?? new TicketPrice[0]).Union(new []{new TicketPrice(name, description, template, price, count) }).ToArray();
			TicketsLeft += count;
			return this;
		}
	}
}