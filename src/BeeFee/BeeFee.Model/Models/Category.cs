using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Category : BaseEntityWithVersion, IModel, IWithName, IWithUrl
	{
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public string Url { get; set; }

		protected Category() : base(null)
		{
		}
	}
}