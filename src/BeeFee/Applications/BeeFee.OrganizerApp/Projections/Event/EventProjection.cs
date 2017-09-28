using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections.Event
{
	public class EventProjection : BaseEntityWithParentAndVersion<CompanyJoinProjection>, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection,
		IUpdateProjection, IRemoveProjection, IWithName, IWithUrl, IWithOwner
	{
		public BaseCategoryProjection Category { get; private set; }
		public EventDateTime DateTime { get; private set; }
		public Address Address { get; private set; }
		public EEventType Type { get; private set; }
		public string Url { get; private set; }
		public string Name { get; private set; }
		public string Email { get; private set; }
		public TicketPrice[] Prices { get; private set; }
		public EventPage Page { get; private set; }

		public BaseUserProjection Owner { get; private set; }

		internal EventProjection Change(string name, string label, string url, string cover, string email, EventDateTime dateTime, Address address, EEventType type,
			BaseCategoryProjection category, TicketPrice[] prices, string html)
		{
			Name = name;
			Url = url.IfNull(name, CommonHelper.UriTransliterate);
			DateTime = dateTime;
			Address = address;
			Type = type;
			Email = email;
			Category = category.HasNotNullArg(nameof(category));
			Prices = prices;
			Page = Page.SetHtml(html).Change(name, label, category.Name, cover, dateTime, address);
			return this;
		}

		public EventProjection(string id, CompanyJoinProjection parent, int version) : base(id, parent, version)
		{
		}
	}
}