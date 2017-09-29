using Core.ElasticSearch.Domain;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;

namespace BeeFee.TestsApp.Projections
{
	internal class NewEvent : BaseNewEntityWithParent<BaseCompanyProjection>, IProjection<Event>
	{
		public BaseCategoryProjection Category { get; set; }
		public string Url { get; set; }
		public string Name { get; set; }
		public BaseUserProjection Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventState State { get; set; }
		public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }

		public NewEvent(BaseCompanyProjection company, BaseUserProjection owner, string name, string url=null) : base(company)
		{
			Owner = owner;
			Name = name.Trim();
			Url = (url ?? CommonHelper.UriTransliterate(name)).ToLowerInvariant();
		}

	}
}