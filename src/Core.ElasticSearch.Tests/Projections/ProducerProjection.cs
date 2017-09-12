using Core.ElasticSearch.Domain;

namespace Core.ElasticSearch.Tests.Projections
{
	public class ProducerProjection : BaseEntity, IProjection<Models.Producer>, IGetProjection, IJoinProjection
	{
		public string Name { get; set; }

		public ProducerProjection(string id) : base(id)
		{
		}
	}
}