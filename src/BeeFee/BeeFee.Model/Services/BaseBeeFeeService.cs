using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using BeeFee.Model.Projections.Jobs;
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

		protected QueryContainer UserQuery<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query = null) where T : class, IWithOwner, ISearchProjection
			=> new QueryContainerDescriptor<T>().Convert(q => q.Term(p => p.Owner, User.HasNotNullArg(x => x.Id, "user").Id) && query?.Invoke(q));

		protected QueryContainer HiddenQuery<T>(bool withHidden = false, QueryContainer query = null) where T : class, IWithHidden, ISearchProjection
			=> withHidden ? query : !Query<T>.Exists(e => e.Field(p => p.Hidden)) && query;

		protected bool ExistsByUrl<T>(string url) where T : class, ISearchProjection, IWithUrl
			=> FilterCount<T>(f => f.Term(p => p.Url, url.HasNotNullArg(nameof(url))))>0;

		public UserName GetUserName(string userId)
			=> User = new UserName(userId);

		protected bool AddJob<T>(T data, DateTime start) where T : struct
			=> Insert(new NewJob<T>(data, start), false);
	}
}