using BeeFee.ModeratorApp.Projections;
using SharpFuncExt;

namespace WebApplication3.Areas.Moderator.Models.Event
{
	public class EventModerateModel
	{
		public string Comment { get; set; }
		public bool Moderate { get; set; }

		public EventPreviewProjection Event {get; private set; }

		public EventModerateModel() { }
		public EventModerateModel(EventPreviewProjection @event) => Init(@event);

		public EventModerateModel Init(EventPreviewProjection @event)
		{
			Event = @event.HasNotNullArg("event");
			return this;
		}
	}
}