using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Event : IModel, IWithVersion, IWithParent<BaseCompanyProjection>, IProjection, IWithName, IWithUrl
	{
		public string Id { get; set; }
		public int Version { get; set; }
		[Keyword]
		public BaseCompanyProjection Parent { get; set; }
		[Keyword]
		public Category Category { get; set; }
		[Keyword]
		public string Url { get; set; }
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public User Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		[Keyword]
		public EEventType Type { get; set; }
		
		/// <summary>
		/// Данные для отображения мероприятия пользователю
		/// </summary>
		public EventPage Page { get; set; }

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
		public Transaction[] Transactions { get; set; }
	}
}