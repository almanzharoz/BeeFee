using System.Linq;
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

		public NewEventTransaction(EventJoinProjection @event)
		{
			Event = @event;
			Company = @event.Parent;
		}
		public NewEventTransaction(NewEvent @event)
		{
			Event = new EventJoinProjection(@event.Id);
			Company = new BaseCompanyProjection(@event.Parent.Id);
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