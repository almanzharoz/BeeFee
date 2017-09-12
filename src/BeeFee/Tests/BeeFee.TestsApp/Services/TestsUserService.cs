using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.TestsApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.TestsApp.Services
{
	public class TestsUserService : BaseBeefeeService
	{
		public TestsUserService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory) : base(loggerFactory, settings, factory,
			null)
		{
		}

		public string AddUser(string email, string password, string name, EUserRole[] roles)
			=> new NewUser(email, name, password) {Roles = roles}.Fluent(x => Insert(x, true)).Id;
	}
}