using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Event : IModel, IWithVersion, IWithParent<BaseCompanyProjection>, IProjection, IWithName, IWithUrl, IWithHidden
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

		[Nested]
		public TicketPrice[] Prices { get; set; }

		public bool Hidden { get; set; }

		[Keyword(Index = false)]
		public string Email { get; set; }

		[Keyword]
		public User Moderator { get; set; }
	}
}