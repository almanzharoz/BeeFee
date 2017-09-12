using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Tests.Models;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Tests.Projections
{
	public struct FullName
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Producer { get; set; }
	}

	public class ProductProjection : BaseEntityWithParentAndVersion<CategoryProjection>, IProjection<Models.Product>, IGetProjection
	{
        public string Name { get; set; }
		public FullName FullName { get; private set; }

		public ProductProjection(string id, CategoryProjection parent, int version) : base(id, parent, version)
		{
		}
	}
}