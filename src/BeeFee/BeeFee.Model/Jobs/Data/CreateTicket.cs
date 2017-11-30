using System;

namespace BeeFee.Model.Jobs.Data
{
	public struct CreateTicket
	{
		public string EventId { get; }
		public string EventTransactionId { get; }
		public string CompanyId { get; }
		public Guid PriceId { get; }

		public string Filename { get; }
		public string Event { get; }
		public string Name { get; }
		public string Date { get; }
		public string Email { get; }
		public string Cover { get; }
		public string Caption { get; }

		public CreateTicket(string eventId, string companyId, string eventTransactionId, Guid priceId, string @event, string name, string date, string email, string cover, string caption, string filename)
		{
			EventId = eventId;
			CompanyId = companyId;
			EventTransactionId = eventTransactionId;
			PriceId = priceId;
			Event = @event;
			Name = name;
			Date = date;
			Email = email;
			Filename = filename;
			Cover = cover;
			Caption = caption;
		}
	}
}