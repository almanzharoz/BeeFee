using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Tests.Models
{
	public class Producer : BaseEntityWithVersion, IModel, IProjection<Producer>, IGetProjection
	{
		public string Name { get; set; }

		public Producer(string id) : base(id)
		{
		}

		public Producer(string id, int version) : base(id, version)
		{
		}
	}

	public class NewProducer : BaseNewEntity, IProjection<Producer>
	{
		public string Name { get; set; }
	}

}