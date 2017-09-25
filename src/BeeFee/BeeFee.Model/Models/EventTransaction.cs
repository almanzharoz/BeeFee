using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class EventTransaction : IModel
	{
		public string Id { get; set; }
		[Keyword]
		public Event Event { get; set; }
		[Keyword]
		public BaseCompanyProjection Company { get; set; }

		/// <summary>
		/// Настройка цен и наличие билетов
		/// </summary>
		[Nested]
		public TicketPrice[] Prices { get; set; }
		/// <summary>
		/// Общее количество оставшихся билетов
		/// </summary>
		public int TicketsLeft { get; set; }

		/// <summary>
		/// Транзакции покупки билетов
		/// </summary>
		[Nested]
		public TicketTransaction[] Transactions { get; set; }

	}
}