using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public abstract class RegisterToEventProjection : BaseEntityWithParent<BaseCompanyProjection>, IProjection<Model.Models.Event>, IUpdateProjection
	{
		public TicketPrice[] Prices { get; }
		public int TicketsLeft { get; }

		public RegisterToEventTransaction[] Transactions { get; }

		protected RegisterToEventProjection(string id) : base(id)
		{
		}
	}
}