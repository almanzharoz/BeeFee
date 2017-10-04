using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Tests.Models;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch.Tests
{
    public class TestService : BaseService<ElasticConnection>
    {
        public TestService(ILoggerFactory loggerFactory, ElasticConnection settings, ElasticScopeFactory<ElasticConnection> factory) : 
			base(loggerFactory, settings, factory)
        {
        }

        public void Clean() => _client.DeleteIndex("*");
        public new void ClearCache() => base.ClearCache();
        
	    public new IReadOnlyCollection<TProjection> Filter<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
		    Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
		    where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
		    => base.Filter<T, TProjection>(query, sort, page, take, load);
    
        public new IReadOnlyCollection<TProjection> Search<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.Search<T, TProjection>(query, sort, page, take, load);

        public new T Get<T>(string id, bool load = true)
            where T : BaseEntity, IProjection, IGetProjection
            => base.GetById<T>(id, load);

	    public new T GetWithVersion<T>(string id, bool load = true)
		    where T : BaseEntityWithVersion, IProjection, IGetProjection
		    => base.GetWithVersionById<T>(id, load);

		public new T Get<T, TParent>(string id, string parent, bool load = true)
		    where T : BaseEntityWithParent<TParent>, IProjection, IGetProjection
		    where TParent : class, IProjection, IJoinProjection
		    => base.GetById<T, TParent>(id, parent, load);

	    public new T GetWithVersion<T, TParent>(string id, string parent, bool load = true)
		    where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
		    where TParent : class, IProjection, IJoinProjection
			=> base.GetWithVersionById<T, TParent>(id, parent, load);

		public new T Get<T>(string id, int version, bool load = true)
		    where T : BaseEntityWithVersion, IProjection, IGetProjection
		    => base.GetById<T>(id, version, load);

	    public new T Get<T, TParent>(string id, string parent, int version, bool load = true)
		    where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
		    where TParent : class, IProjection, IJoinProjection
			=> base.GetById<T, TParent>(id, parent, version, load);

		public new int FilterCount<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IProjection
            => base.FilterCount<T>(query);

        public bool Insert<T>(T entity) where T : BaseNewEntity, IProjection
			=> base.Insert<T>(entity, true);

		public new T Insert<TNew, T>(TNew entity) where TNew : BaseNewEntity, IProjection
		    where T : BaseEntity, IProjection, IGetProjection
		    => base.Insert<TNew, T>(entity);

		public new T InsertWithId<TNew, T>(TNew entity) where TNew : BaseNewEntityWithId, IProjection
			where T : BaseEntity, IProjection, IGetProjection
			=> base.InsertWithId<TNew, T>(entity);

		public new T InsertWithVersion<TNew, T>(TNew entity) where TNew : BaseNewEntity, IProjection
		    where T : BaseEntityWithVersion, IProjection, IGetProjection
		    => base.InsertWithVersion<TNew, T>(entity);

		public new T InsertWithIdAndVersion<TNew, T>(TNew entity) where TNew : BaseNewEntityWithId, IProjection
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> base.InsertWithIdAndVersion<TNew, T>(entity);

		public bool InsertWithParent<T, TParent>(T entity)
            where T : BaseNewEntityWithParent<TParent>
			where TParent : class, IProjection, IJoinProjection
			=> base.Insert<T, TParent>(entity, true);

        public bool UpdateWithVersion<T>(T entity) where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> base.UpdateWithVersion<T>(entity, true);

        public int Update<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update)
            where T : class, IEntity, IWithVersion, IProjection, IUpdateProjection
			=> base.Update<T>(query, update, true);

        public bool Remove<T>(T entity) 
			where T : BaseEntity, IProjection, IRemoveProjection
			=> base.Remove<T>(entity, true);

	    public bool RemoveWithVersion<T>(T entity)
		    where T : BaseEntityWithVersion, IProjection, IRemoveProjection
		    => base.RemoveWithVersion<T>(entity, true);

		public int Remove<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IRemoveProjection
			=> base.Remove<T>(query, true);

		public new Pager<TProjection> FilterPager<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.FilterPager<T, TProjection>(query, page, take, sort, load);

        public new Pager<TProjection> SearchPager<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.SearchPager<T, TProjection>(query, page, take, sort, load);

        public new IReadOnlyCollection<KeyValuePair<TProjection, string>> CompletionSuggest<T, TProjection>(
            Func<CompletionSuggesterDescriptor<T>, ICompletionSuggester> suggester, int page = 0, int take = 0,
            bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
            where T : class, IModel
            => base.CompletionSuggest<T, TProjection>(suggester, page, take, load);
    }
}