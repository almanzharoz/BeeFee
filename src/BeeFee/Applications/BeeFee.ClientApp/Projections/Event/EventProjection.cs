using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventProjection : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		public string Url { get; }

		public string Name { get; }

		public BaseCategoryProjection Category { get; }

		public EEventType Type { get; }

		public TicketPrice[] Prices { get; }

		public int TicketsLeft { get; }
		
		public EventPage Page { get; }

		public EventProjection(string id, BaseCompanyProjection company, string url, string name, BaseCategoryProjection category, EEventType type, TicketPrice[] prices, EventPage page, int ticketsLeft) : base(id, company)
		{
			Url = url;
			Name = name;
			Category = category;
			Type = type;
			Prices = prices;
			Page = page;
			TicketsLeft = ticketsLeft;
		}
	}
}