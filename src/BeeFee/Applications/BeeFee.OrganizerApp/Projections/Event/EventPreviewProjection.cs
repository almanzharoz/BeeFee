using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Event
{
	public class EventPreviewProjection : BaseEntityWithParent<BaseCompanyProjection>, IEventPageProjection, IGetProjection
	{
		public string Url { get; }

		public string Name { get; }

		public BaseCategoryProjection Category { get; }

		public EEventState State { get; }

		public EventPage Page { get; }

		public EventPreviewProjection(string id, BaseCompanyProjection company, string url, string name, BaseCategoryProjection category, EEventState state, EventPage page) : base(id, company)
		{
			Url = url;
			Name = name;
			Category = category;
			State = state;
			Page = page;
		}
	}
}