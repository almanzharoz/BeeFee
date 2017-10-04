using System.Collections.Generic;
using Core.ElasticSearch;
using BeeFee.AdminApp.Projections;
using BeeFee.AdminApp.Projections.Category;
using BeeFee.Model;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;
using CategoryProjection = BeeFee.AdminApp.Projections.Category.CategoryProjection;

namespace BeeFee.AdminApp.Services
{
	public class CategoryService : BaseBeefeeService
	{
		public CategoryService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public bool Add(string name, string url=null)
			=> Insert(new NewCategory(url, name.Trim()), true);

		public bool Remove(string id) => Remove<CategoryProjection>(id, true);

		public bool Rename(string id, int version, string name, string url)
			=> UpdateById<CategoryProjection>(id, version, x => x.Rename(name, url), true);

		public IReadOnlyCollection<CategoryProjection> GetAllCategories()
			=> Filter<Category, CategoryProjection>(null, s => s.Ascending(p => p.Name));
	}
}