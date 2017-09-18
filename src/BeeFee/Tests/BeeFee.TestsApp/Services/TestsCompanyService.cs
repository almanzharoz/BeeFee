using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.TestsApp.Projections;
using Core.ElasticSearch.Domain;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.TestsApp.Services
{
	public class TestsCompanyService : BaseBeefeeService
	{
		public TestsCompanyService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user)
			: base(loggerFactory, settings, factory, user)
		{
		}

		public string Add(string name, string url = null)
			=> new NewCompany(url, name.Trim(), Get<BaseUserProjection>(User.Id)).Fluent(x => Insert(x, true)).Id;

		public T GetCompany<T>(string id) where T : BaseEntity, IProjection<Company>, IGetProjection
			=> Get<T>(id, true);
	}
}