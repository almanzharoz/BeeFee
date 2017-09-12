using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventAddressProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, ISearchProjection
	{
		public Address Address { get; }

		public EventAddressProjection(string id, int version, Address address) : base(id, version)
		{
			Address = address;
		}
	}
}