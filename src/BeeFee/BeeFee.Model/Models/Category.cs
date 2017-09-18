using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Category : IModel, IWithVersion, IWithName, IWithUrl
	{
		public string Id { get; set; }
		public int Version { get; set; }
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public string Url { get; set; }
	}
}