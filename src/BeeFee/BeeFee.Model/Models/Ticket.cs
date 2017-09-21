using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.Model.Models
{
	public class Ticket : BaseEntity
	{
		[Keyword]
		public Event Event { get; set; }

		public Ticket(string id) : base(id)
		{
		}
	}
}