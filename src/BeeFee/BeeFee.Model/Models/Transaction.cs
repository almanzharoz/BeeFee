using BeeFee.Model.Embed;
using System;
using Nest;

namespace BeeFee.Model.Models
{
	//TODO: Переопределить проекции для изменения статуса и для создания
	public class Transaction
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

		public Transaction(Guid id, Guid priceId, DateTime date, Contact contact, float sum, ETransactionType type, ETransactionState state)
		{
			Id = id;
			PriceId = priceId;
			Date = date;
			Contact = contact;
			Sum = sum;
			Type = type;
			State = state;
		}
	}
}