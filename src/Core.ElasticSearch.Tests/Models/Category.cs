using System;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Tests.Projections;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Tests.Models
{
	public class Category : BaseEntityWithVersion, IModel, IProjection<Category>, IGetProjection, IUpdateProjection, ISearchProjection, IRemoveProjection
	{
        [Keyword]
		public CategoryProjection Top { get; set; }
		public string Name { get; set; }
        public DateTime CreatedOnUtc { get; set; }

		public Category(string id, int version) : base(id, version)
		{
		}
	}

	public class NewCategory : BaseNewEntity, IProjection<Category>
	{
		[Keyword]
		public CategoryJoin Top { get; set; }
		public string Name { get; set; }
		public DateTime CreatedOnUtc { get; set; }

		public NewCategory() : base() { }
	}

	public class NewCategoryWithId : BaseNewEntityWithId, IProjection<Category>
	{
		[Keyword]
		public CategoryJoin Top { get; set; }
		public string Name { get; set; }
		public DateTime CreatedOnUtc { get; set; }

		public NewCategoryWithId(string id) : base(id) { }
	}

}