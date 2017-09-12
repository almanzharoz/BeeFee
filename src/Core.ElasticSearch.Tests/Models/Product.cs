using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Tests.Projections;
using Nest;

namespace Core.ElasticSearch.Tests.Models
{
	public class Product : BaseEntityWithParentAndVersion<CategoryProjection>, IModel, IProjection<Product>, IGetProjection,  ISearchProjection
    {
		public string Name { get; set; }
		[Keyword]
		public ProducerProjection Producer { get; set; }
		public Projections.FullName FullName { get; set; }

	    [Completion]
	    public string Title { get; set; }

	    public Product(string id, CategoryProjection parent, int version) : base(id, parent, version)
	    {
	    }
    }

	public class NewProduct : BaseNewEntityWithParent<CategoryProjection>, IProjection<Product>
	{
		public string Name { get; set; }
		[Keyword]
		public ProducerProjection Producer { get; set; }
		public Projections.FullName FullName { get; set; }

		[Completion]
		public string Title { get; set; }

		public NewProduct(CategoryProjection parent) : base(parent)
		{
		}
	}
}