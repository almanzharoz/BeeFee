using BeeFee.OrganizerApp.Projections.Event;
using Core.ElasticSearch;
using WebApplication3.Models;

namespace WebApplication3.Areas.Org.Models.Company
{
	public class EventsFilter : PagerFilter<EventProjection>
	{
		public EventsFilter() : base(50)
		{
		}

		public EventsFilter Load(Pager<EventProjection> items)
		{
			LoadItems(items);
			return this;
		}
	}
}