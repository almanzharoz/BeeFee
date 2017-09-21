using System;
using BeeFee.Model.Embed;

namespace BeeFee.ClientApp.Projections.Event
{
    public class RegisterToEventTransaction
    {
		public Guid PriceId { get; }
		public DateTime Date { get; }
		public Contact Contact { get; }
		public float Sum { get; }
		public ETransactionType Type { get; }
		public ETransactionState State { get; }

		public RegisterToEventTransaction(Guid priceId, DateTime date, Contact contact, float sum, ETransactionType type)
		{
			PriceId = priceId;
			Date = date;
			Contact = contact;
			Sum = sum;
			Type = type;
			State = ETransactionState.New;
		}
    }
}
