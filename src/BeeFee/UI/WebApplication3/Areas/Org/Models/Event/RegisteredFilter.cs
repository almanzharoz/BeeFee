using BeeFee.OrganizerApp.Projections.Event;
using Core.ElasticSearch;
using WebApplication3.Models;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class RegisteredFilter : PagerFilter<EventTicketTransaction>
	{
		public RegisteredFilter() : base(50)
		{
		}

		public RegisteredFilter Load(Pager<EventTicketTransaction> items)
		{
			LoadItems(items);
			return this;
		}
	}
}