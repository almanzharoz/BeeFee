using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Projections;
using BeeFee.TestsApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.TestsApp.Services
{
	public class TestsCaterogyService : BaseBeefeeService
	{
		public TestsCaterogyService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user)
			: base(loggerFactory, settings, factory, user)
		{
		}

		public string Add(string name, string url = null)
			=> new NewCategory(url, name.Trim()).Fluent(x => Insert(x, true)).Id;
	}
}