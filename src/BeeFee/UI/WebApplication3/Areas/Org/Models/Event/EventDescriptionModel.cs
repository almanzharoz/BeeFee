
using BeeFee.OrganizerApp.Projections.Event;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class EventDescriptionModel
	{
		public string Html { get; set; }

		public EventDescriptionModel() { }

		public EventDescriptionModel(EventProjection @event)
		{
			Html = @event.Page.Html;
		}
	}
}