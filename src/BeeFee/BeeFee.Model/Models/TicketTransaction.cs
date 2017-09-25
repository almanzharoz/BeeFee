using BeeFee.Model.Embed;
using System;
using Nest;

namespace BeeFee.Model.Models
{
	//TODO: Переопределить проекции для изменения статуса и для создания
	public abstract class TicketTransaction
	{
		[Keyword]
		public Guid Id { get; }
		[Keyword]
		public Guid PriceId { get; }
		[Keyword]
		public DateTime Date { get; }
		public Contact Contact { get; }
		public float Sum { get; }
		[Keyword]
		public ETransactionType Type { get; }
		[Keyword]
		public ETransactionState State { get; }
		[Keyword]
		public Ticket Ticket { get; }

	}
}