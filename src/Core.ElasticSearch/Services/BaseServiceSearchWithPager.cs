using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected Pager<TProjection> FilterPager<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel // Нужно для SortDescriptor<T>, чтобы использовать любое поля для сортировки, а не только поля из проекции
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query(new QueryContainerDescriptor<T>()))))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip((page > 0 ? page - 1 : 0) * take))),
						r => new Pager<TProjection>(page, take, (int) r.Total, r.Documents.If(load, Load)),
						RepositoryLoggingEvents.ES_SEARCH));

		protected Pager<TProjection> SearchPager<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
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
								.IfNotNull(take, y => y.Take(take).Skip((page > 0 ? page - 1 : 0) * take))),
						r => new Pager<TProjection>(page, take, (int) r.Total, r.Documents.If(load, Load)),
						RepositoryLoggingEvents.ES_SEARCH));

		protected Task<Pager<TProjection>> SearchPagerAsync<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip((page > 0 ? page - 1 : 0) * take))),
						r => new Pager<TProjection>(page, take, (int)r.Total, r.Documents.If(load, Load)),
						RepositoryLoggingEvents.ES_SEARCH));
	}
}