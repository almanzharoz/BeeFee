using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ModeratorApp.Projections
{
	public class EventModeratorGridItem : BaseEntityWithParentAndVersion<BaseCompanyProjection>, IProjection<Event>, ISearchProjection
	{
		public string Name { get; }
		public string Url { get; }
		public EventDateTime DateTime { get; }
		public Address Address { get; }
		public BaseCategoryProjection Category { get; }

		public EventModeratorGridItem(string id, BaseCompanyProjection parent, int version,
			string name, string url, EventDateTime datetime, Address address, BaseCategoryProjection category) : base(id, parent, version)
		{
			Name = name;
			Url = url;
			DateTime = datetime;
			Address = address;
			Category = category;
		}
	}
}