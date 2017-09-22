﻿using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	/// <summary>
	/// Для обновления при попытке регистрации на мероприятие
	/// </summary>
	public abstract class RegisterToEventProjection : BaseEntity, IProjection<Model.Models.EventTransaction>, IUpdateProjection
	{
		public EventJoinProjection Event { get; }
		public BaseCompanyProjection Company { get; }
		public TicketPrice[] Prices { get; }
		public int TicketsLeft { get; }

		public RegisterToEventTransaction[] Transactions { get; }

		protected RegisterToEventProjection(string id) : base(id)
		{
		}
	}
	public abstract class EventJoinProjection : BaseEntity, IProjection<Model.Models.Event>, IJoinProjection
	{
		protected EventJoinProjection(string id) : base(id)
		{
		}
	}
}