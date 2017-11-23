using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ElasticSearch;
using BeeFee.AdminApp.Projections.Category;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;

namespace BeeFee.AdminApp.Services
{
	public class CategoryService : BaseBeefeeService
	{
		public CategoryService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public Task<bool> Add(string name, string url=null)
			=> InsertAsync(new NewCategory(url, name.Trim()), true);

		public Task<bool> Remove(string id, int version) => RemoveAsync<CategoryProjection>(id, version, true);

		public Task<bool> Rename(string id, int version, string name, string url)
			=> UpdateByIdAsync<CategoryProjection>(id, version, x => x.Rename(name, url), true);

		public Task<CategoryProjection> GetCategory(string id, int version)
			=> GetByIdAsync<CategoryProjection>(id, version);

		public IReadOnlyCollection<CategoryProjection> GetAllCategories()
			=> Filter<Category, CategoryProjection>(null, s => s.Ascending(p => p.Name));
	}
}