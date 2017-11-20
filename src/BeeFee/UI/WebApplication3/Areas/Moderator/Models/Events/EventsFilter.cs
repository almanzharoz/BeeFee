using BeeFee.ModeratorApp.Projections;
using Core.ElasticSearch;
using WebApplication3.Models;

namespace WebApplication3.Areas.Moderator.Models.Events
{
	public class EventsFilter : PagerFilter<EventModeratorGridItem>
	{
		public EventsFilter() : base(10)
		{
		}

		public EventsFilter Load(Pager<EventModeratorGridItem> items)
		{
			LoadItems(items);
			return this;
		}
	}
}