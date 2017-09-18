using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventGridItem : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		public EventGridItemPage Page { get; }

		public string Url { get; }

		public EventGridItem(string id, BaseCompanyProjection company, string url, EventGridItemPage page) : base(id, company)
		{
			Page = page;
			Url = url;
		}
	}
}