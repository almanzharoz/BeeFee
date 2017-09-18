using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventAddressProjection : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Model.Models.Event>, ISearchProjection
	{
		public Address Address { get; }

		public EventAddressProjection(string id, BaseCompanyProjection company, Address address) : base(id, company)
		{
			Address = address;
		}
	}
}