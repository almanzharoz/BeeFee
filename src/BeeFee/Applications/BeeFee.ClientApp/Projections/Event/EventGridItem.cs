using BeeFee.Model.Interfaces;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventGridItem : BaseEntity, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		public EventGridItemPage Page { get; }

		public string Url { get; }

		public EventGridItem(string id, string url, EventGridItemPage page) : base(id)
		{
			Page = page;
			Url = url;
		}
	}
}