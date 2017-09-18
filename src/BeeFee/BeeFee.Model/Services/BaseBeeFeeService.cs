using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
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

		protected QueryContainer UserQuery<T>(QueryContainer query = null) where T : class, IWithOwner, ISearchProjection
			=> Query<T>.Term(p => p.Owner, User.HasNotNullArg(x => x.Id, "user").Id) && query;

		protected bool ExistsByUrl<T>(string url) where T : class, ISearchProjection, IWithUrl
			=> FilterCount<T>(f => f.Term(p => p.Url, url.HasNotNullArg(nameof(url))))>0;

		public UserName UseUserName(string userId)
			=> User = new UserName(userId);
	}
}