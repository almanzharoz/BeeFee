using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using BeeFee.Model;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.Model
{
	public abstract class BaseBeefeeService : BaseService<BeefeeElasticConnection>
	{
		protected UserName User;

		protected BaseBeefeeService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory)
		{
			User = user;
		}

		protected QueryContainer UserQuery<T>(QueryContainer query) where T : class, IWithOwner, ISearchProjection
			=> Query<T>.Term(p => p.Owner, User.HasNotNullArg(x => x.Id, "user").Id) && query;

		public UserName UseUserName(string userId)
			=> User = new UserName(userId);
	}
}