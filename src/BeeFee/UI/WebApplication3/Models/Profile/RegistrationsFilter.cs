using BeeFee.ClientApp.Projections.Event;
using Core.ElasticSearch;

namespace WebApplication3.Models.Profile
{
	public class RegistrationsFilter : PagerFilter<EventTicketTransaction>
	{
		public RegistrationsFilter() : base(10)
		{
		}

		public RegistrationsFilter Load(Pager<EventTicketTransaction> items)
		{
			LoadItems(items);
			return this;
		}
	}
}