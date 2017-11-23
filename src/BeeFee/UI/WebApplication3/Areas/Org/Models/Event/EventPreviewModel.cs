using BeeFee.OrganizerApp.Projections.Event;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class EventPreviewModel : IEventEditModel
	{
		public EventPreviewProjection Event { get; }

		public EventPreviewModel(EventPreviewProjection @event)
		{
			Event = @event;
		}
	}
}