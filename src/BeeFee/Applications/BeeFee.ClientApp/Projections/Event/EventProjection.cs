using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventProjection : BaseEntity, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		public string Url { get; }

		public string Name { get; }

		public BaseCategoryProjection Category { get; }

		public EEventType Type { get; }

		public TicketPrice[] Prices { get; }

		public EventPage Page { get; }

		public EventProjection(string id, string url, string name, BaseCategoryProjection category, EEventType type, TicketPrice[] prices, EventPage page) : base(id)
		{
			Url = url;
			Name = name;
			Category = category;
			Type = type;
			Prices = prices;
			Page = page;
		}
	}
}