using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.TestsApp.Projections
{
    public class FullEventTransaction : BaseEntityWithParent<BaseCompanyProjection>, IProjection<EventTransaction>, ISearchProjection
    {
		public EventJoinProjection Event { get; private set; }
		public BaseCompanyProjection Company { get; private set; }

		public TicketPrice[] Prices { get; private set; }

		public int TicketsLeft { get; private set; }

		public Transaction[] Transactions { get; private set; }

		public FullEventTransaction(string id) : base(id)
		{
		}
	}
}
