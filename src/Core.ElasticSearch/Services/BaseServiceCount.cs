using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected IReadOnlyCollection<KeyValuePair<TProjection, int>> SearchWithScore<T, TProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 100, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Hits.Select(x => new KeyValuePair<TProjection, int>(x.Source, (int)(x.Score>1 && x.Score<2?0:x.Score))).ToArray().If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected int FilterCount<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IEntity
			=> Try(
				c => c.Count<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);

		protected Task<int> FilterCountAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IEntity
			=> TryAsync(
				c => c.CountAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);

		protected int SearchCount<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IEntity
			=> Try(
				c => c.Count<T>(d => d.Query(query)
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);

		protected Task<int> SearchCountAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query)
			where T : class, IEntity
			=> TryAsync(
				c => c.CountAsync<T>(d => d.Query(query)
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);
	}
}