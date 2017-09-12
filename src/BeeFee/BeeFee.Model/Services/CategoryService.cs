using System.Collections.Generic;
using System.Linq;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Microsoft.Extensions.Logging;

namespace BeeFee.Model.Services
{
	public class CategoryService : BaseBeefeeService
	{
		public CategoryService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public IReadOnlyCollection<T> GetAllCategories<T>() where T : BaseEntity, IProjection<Category>, ISearchProjection
			=> Filter<Category, T>(null, s => s.Ascending(p => p.Name));

		public T GetCategoryByUrl<T>(string url) where T : BaseEntity, IProjection<Category>, ISearchProjection
			=> Filter<Category, T>(q => q.Term(t => t.Field(p => p.Url).Value(url.ToLowerInvariant()))).SingleOrDefault();
	}
}