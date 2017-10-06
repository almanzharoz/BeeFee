using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Interfaces
{
	public interface IEventPageProjection : IProjection<Model.Models.Event>, IWithParent<BaseCompanyProjection>, IWithName, IWithUrl
	{
		BaseCategoryProjection Category { get; }

		EEventState State { get; }

		TicketPrice[] Prices { get; }

		EventPage Page { get; }

	}
}