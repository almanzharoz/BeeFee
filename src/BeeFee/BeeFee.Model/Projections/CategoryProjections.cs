using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections
{
	public class BaseCategoryProjection : BaseEntity, IProjection<Category>, IWithName, IJoinProjection, IGetProjection
	{
		public string Name { get; private set; }

		public BaseCategoryProjection(string id) : base(id)
		{
		}
	}

	public class CategoryProjection : BaseEntity, IProjection<Category>, IWithName, IWithUrl, ISearchProjection
	{
		public string Name { get; private set; }
		public string Url { get; private set; }

		public CategoryProjection(string id) : base(id)
		{
		}
	}
}