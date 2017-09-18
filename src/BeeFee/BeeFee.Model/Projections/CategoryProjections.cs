using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections
{
	public class BaseCategoryProjection : BaseEntity, IProjection<Category>, IWithName, IJoinProjection, IGetProjection, ISearchProjection
	{
		public string Name { get; private set; }
		public string Url { get; private set; }

		public BaseCategoryProjection(string id) : base(id)
		{
		}
	}

}