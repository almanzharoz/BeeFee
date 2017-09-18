using BeeFee.Model.Interfaces;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections
{
	public class BaseCompanyProjection : BaseEntity, IProjection<Model.Models.Company>, IJoinProjection, IGetProjection, IWithName, IWithUrl
	{
		public string Url { get; private set; }
		public string Name { get; private set; }

		public BaseCompanyProjection(string id) : base(id)
		{
		}

	}

}