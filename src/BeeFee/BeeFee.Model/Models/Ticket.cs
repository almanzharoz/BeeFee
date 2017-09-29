using System;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Ticket : IModel
	{
		public string Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Used { get; set; }

		[Keyword]
		public Event Event { get; set; }
		[Keyword]
		public User User { get; set; }

		public Contact Contact { get; set; }

	}
}