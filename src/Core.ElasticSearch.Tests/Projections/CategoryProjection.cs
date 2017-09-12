using Core.ElasticSearch.Domain;
using Nest;

namespace Core.ElasticSearch.Tests.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Models.Category>, IGetProjection, ISearchProjection, IJoinProjection
	{
		[Keyword]
		public Projections.CategoryJoin Top { get; private set; }
		public string Name { get; private set; }

		public CategoryProjection(string id) : base(id)
		{
		}
		//public CategoryProjection(string id, string name, CategoryJoin top) : base(id)
		//{
		//	Name = name;
		//	Top = top;
		//}
	}

	public class CategoryJoin : BaseEntityWithVersion, IProjection<Models.Category>, IGetProjection, IJoinProjection
	{
		[Keyword]
		public Projections.CategoryJoin Top { get; private set; }
		public string Name { get; private set; }

		public CategoryJoin(string id) : base(id)
		{
		}
	}
}