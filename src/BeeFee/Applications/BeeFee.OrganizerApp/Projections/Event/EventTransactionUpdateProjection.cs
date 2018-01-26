using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Event
{
	internal class EventTransactionUpdateProjection : BaseEntity, IProjection<EventTransaction>, ISearchProjection, IUpdateProjection
	{
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }
		public EventDateTime DateTime { get; private set; }

		public EventTransactionUpdateProjection(string id, BaseCompanyProjection company, EventJoinProjection @event, EventDateTime dateTime) : base(id)
		{
			Company = company;
			Event = @event;
			DateTime = dateTime;
		}

		public EventTransactionUpdateProjection Update(EventDateTime dateTime)
		{
			DateTime = new EventDateTime(dateTime.Start.ToUniversalTime(), dateTime.Finish.ToUniversalTime());
			return this;
		}
	}
}