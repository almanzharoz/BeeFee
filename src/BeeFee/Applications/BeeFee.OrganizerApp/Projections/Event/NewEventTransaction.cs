using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Event
{
	internal class NewEventTransaction : BaseNewEntity, IProjection<EventTransaction>
	{
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }
		public EventDateTime DateTime { get; }

		//public NewEventTransaction(EventJoinProjection @event)
		//{
		//	Event = @event;
		//	Company = @event.Parent;
		//	DateTime = @event.
		//}
		public NewEventTransaction(NewEvent @event)
		{
			Event = new EventJoinProjection(@event.Id);
			Company = new BaseCompanyProjection(@event.Parent.Id);
			DateTime = new EventDateTime(@event.DateTime.Start.ToUniversalTime(), @event.DateTime.Finish.ToUniversalTime());
		}

	}

	public class EventJoinProjection : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Model.Models.Event>, IJoinProjection, IGetProjection
	{
		public string Url { get; private set; }
		public EventJoinProjection(string id) : base(id)
		{
		}
	}
}