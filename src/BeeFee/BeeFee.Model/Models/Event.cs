using System;
using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using Nest;

namespace BeeFee.Model.Models
{
	/// <summary>
	/// Модель мероприятия для фильтрации и отображения ячеек
	/// </summary>
	public abstract class Event : BaseEntityWithVersion, IModel, IProjection, IWithName
	{
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
		public TicketPrice[] Prices { get; set; }

		public EventPage Page { get; set; }

		protected Event() : base(null) { }
	}
}