using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

		protected Task<Pager<TProjection>> FilterPagerAsync<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel // Нужно для SortDescriptor<T>, чтобы использовать любое поля для сортировки, а не только поля из проекции
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query(new QueryContainerDescriptor<T>()))))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip((page > 0 ? page - 1 : 0) * take))),
						r => new Pager<TProjection>(page, take, (int)r.Total, r.Documents.If(load, Load)),
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

		public Task<Pager<TInnerProjection>> FilterNestedAsync<T, TNested, TProjection, TInnerProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Expression<Func<T, TNested[]>> path,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null,
			int page = 0, int take = 0, bool load = true)
			where TProjection : class, ISearchProjection, IProjection<T>
			where T : class, IModel
			where TInnerProjection : class
			=> TryAsync(
				c => c.SearchAsync<T, TProjection>(
					x => x
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						//.Source(s => s.Includes(f => f.Fields(projection.Fields)))
						.Query(q => query(q) && q.Nested(n => n.Path(path).Query(iq => iq.MatchAll())
										.InnerHits(h => h.Name("innerhits").IfNotNull(sort, y => y.Sort(sort)).IfNotNull(take, y => y.Size(take).From(page * take)))))
						.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())),
				r => new Pager<TInnerProjection>(page, take, (int)r.Total, r.Hits.SelectMany(s => s.InnerHits.Values.SelectMany(h => h.Documents<TInnerProjection>())).ToArray().If(load, Load)),
				RepositoryLoggingEvents.ES_SEARCH);

		public Task<Pager<TInnerProjection>> FilterNestedAsync<T, TNested, TProjection, TInnerProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Expression<Func<T, TNested[]>> path,
			Func<QueryContainerDescriptor<T>, QueryContainer> nestedQuery,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null,
			int page = 0, int take = 0, bool load = true)
			where TProjection : class, ISearchProjection, IProjection<T>
			where T : class, IModel
			where TInnerProjection : class
			=> TryAsync(
				c => c.SearchAsync<T, TProjection>(
					x => x
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						//.Source(s => s.Includes(f => f.Fields(projection.Fields)))
						.Query(q => query(q) && q.Nested(n => n.Path(path).Query(iq => iq.Bool(b => b.Filter(nestedQuery(new QueryContainerDescriptor<T>()))))
										.InnerHits(h => h.Name("innerhits").IfNotNull(sort, y => y.Sort(sort)).IfNotNull(take, y => y.Size(take).From(page * take)))))
						.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())),
				r => new Pager<TInnerProjection>(page, take, (int)r.Total, r.Hits.SelectMany(s => s.InnerHits.Values.SelectMany(h => h.Documents<TInnerProjection>())).ToArray().If(load, Load)),
				RepositoryLoggingEvents.ES_SEARCH);
	}
}