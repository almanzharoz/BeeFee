using Core.ElasticSearch.Domain;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Nest;

namespace BeeFee.TestsApp.Projections
{
	internal class NewEvent : BaseNewEntity, IProjection<Event>
	{
		[Keyword]
		public BaseCategoryProjection Category { get; set; }
		[Keyword]
		public string Url { get; set; }
		public string Name { get; set; }
		[Keyword]
		public BaseUserProjection Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }

		public NewEvent() { }
		public NewEvent(BaseUserProjection owner, string name, string url=null) // for new event
		{
			Owner = owner;
			Name = name.Trim();
			Url = (url ?? CommonHelper.UriTransliterate(name)).ToLowerInvariant();
		}

	}
}