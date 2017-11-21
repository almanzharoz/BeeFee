using System;
using System.ComponentModel.DataAnnotations;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.Model.Helpers;
using BeeFee.Model.Projections;
using SharpFuncExt;

namespace WebApplication3.Models.Event
{
	public class EventPageModel
	{
		public EventPageModel()
		{
		}

		public EventPageModel(EventProjection @event, EventTransactionPricesProjection eventTransaction, string name, string email, string phone)
		{
			Event = @event.HasNotNullEntityWithParent<EventProjection, BaseCompanyProjection>(nameof(@event));
			EventTransaction = eventTransaction.HasNotNullEntity(nameof(eventTransaction));
			Name = name;
			Email = email;
			Phone = phone;
		}

		public EventPageModel(EventProjection @event, EventTransactionPricesProjection eventTransaction, string name, string email, string phone, Guid ticketId)
			:this(@event, eventTransaction, name, email, phone)
		{
			TicketId = ticketId;
		}

		public EventPageModel(EventProjection @event, EventTransactionPricesProjection eventTransaction, bool? registered)
			: this(@event, eventTransaction, null, null, null)
		{
			Registered = registered;
		}

		public EventProjection Event { get; }

		public EventTransactionPricesProjection EventTransaction { get; }

		public bool? Registered { get; }

		[Required]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[Phone]
		public string Phone { get; set; }

		public Guid TicketId { get; set; }
	}
}