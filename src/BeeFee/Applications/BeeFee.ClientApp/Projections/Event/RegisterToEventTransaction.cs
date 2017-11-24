﻿using System;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Nest;

namespace BeeFee.ClientApp.Projections.Event
{
    public class RegisterToEventTransaction
    {
		public Guid PriceId { get; }
		public DateTime Date { get; }
		//public BaseUserProjection User { get; }
		public Contact Contact { get; }
		public float Sum { get; }
		public ETransactionType Type { get; }
		public ETransactionState State { get; }
		public string SessionId { get; }

		public RegisterToEventTransaction(Guid priceId, DateTime date, Contact contact/*, BaseUserProjection user*/, float sum, ETransactionType type, string sessionId)
		{
			PriceId = priceId;
			Date = date;
			Contact = contact;
			//User = user;
			Sum = sum;
			Type = type;
			State = ETransactionState.New;
			SessionId = sessionId;
		}
    }
}
