using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected IReadOnlyCollection<TProjection> Filter<T, TProjection>(
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
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected Task<IReadOnlyCollection<TProjection>> FilterAsync<T, TProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 100, bool load = true)
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
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		public IReadOnlyCollection<TInnerProjection> FilterNested<T, TNested, TProjection, TInnerProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Expression<Func<T, TNested[]>> path,
			Func<QueryContainerDescriptor<T>, QueryContainer> innerQquery,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null,
			int page = 0, int take = 0, bool load = true)
			where TProjection : class, ISearchProjection, IProjection<T>
			where T : class, IModel
			where TInnerProjection : class
			=> Try(
				c => c.Search<T, TProjection>(
					x => x
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						//.Source(s => s.Includes(f => f.Fields(projection.Fields)))
						.Query(q => query(q) && q.Nested(n => n.Path(path).Query(nq => nq.Bool(b => b.Filter(innerQquery)))
						.InnerHits(h => h.IfNotNull(sort, y => y.Sort(sort)).IfNotNull(take, y => y.Size(take).From(page * take)))))
						.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())),
				r => r.Hits.SelectMany(s => s.InnerHits.Values.SelectMany(h => h.Documents<TInnerProjection>())).ToArray()
					.If(load, Load),
				RepositoryLoggingEvents.ES_SEARCH);

		public IReadOnlyCollection<TInnerProjection> FilterNested<T, TNested, TProjection, TInnerProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Expression<Func<T, TNested[]>> path,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null,
			int page = 0, int take = 0, bool load = true)
			where TProjection : class, ISearchProjection, IProjection<T>
			where T : class, IModel
			where TInnerProjection : class
			=> Try(
				c => c.Search<T, TProjection>(
					x => x
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						//.Source(s => s.Includes(f => f.Fields(projection.Fields)))
						.Query(q => query(q) && q.Nested(n => n.Path(path).Query(iq => iq.MatchAll())
										.InnerHits(h => h.Name("innerhits").IfNotNull(sort, y => y.Sort(sort)).IfNotNull(take, y => y.Size(take).From(page * take)))))
						.Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())),
				r => r.Hits.SelectMany(s => s.InnerHits.Values.SelectMany(h => h.Documents<TInnerProjection>())).ToArray()
					.If(load, Load),
				RepositoryLoggingEvents.ES_SEARCH);

		protected IReadOnlyCollection<TProjection> Search<T, TProjection>(
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
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<T> Filter<T>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 100, bool load = true)
			where T : class, IProjection, ISearchProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Search<T>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, T, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected Task<IReadOnlyCollection<T>> FilterAsync<T>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 100, bool load = true)
			where T : class, IProjection, ISearchProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<T>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, T, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<T> Search<T>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 100, bool load = true)
			where T : class, IProjection, ISearchProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Search<T>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.Is<SearchDescriptor<T>, T, IWithVersion>(y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<KeyValuePair<TProjection, string>> CompletionSuggest<T, TProjection>(
           Func<CompletionSuggesterDescriptor<T>, ICompletionSuggester> suggester, int page = 0, int take = 100, bool load = true)
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
                               .IfNotNull(suggester, y => y.Suggest(ss => ss.Completion("my-completion-suggest", suggester)))
							   .Is<SearchDescriptor<T>, TProjection, IWithVersion>(y => y.Version())
							   .IfNotNull(take, y => y.Take(take).Skip(page * take))),
                       r => r.Suggest["my-completion-suggest"].SelectMany(f => f.Options).Select(x => new KeyValuePair<TProjection, string>(x.Source, x.Text)).ToArray().If(load, Load),
                       RepositoryLoggingEvents.ES_SEARCH));

	}
}