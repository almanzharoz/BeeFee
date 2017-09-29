using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
    {
		protected bool Remove<T>(T entity, bool refresh)
			where T : BaseEntity, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected bool RemoveWithVersion<T>(T entity, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Version(entity.Version.HasNotNullArg("version"))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Version: {entity.Version})");

		protected bool Remove<T, TParent>(T entity, bool refresh)
			where T : BaseEntityWithParent<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection, IJoinProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(entity.Parent.HasNotNullArg(p => p.Id, "parent").Id)
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Parent: {entity.Parent})");

		protected bool RemoveWithVersion<T, TParent>(T entity, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection, IJoinProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(entity.Parent.HasNotNullArg(p => p.Id, "parent").Id)
					.Version(entity.Version.HasNotNullArg("version"))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Parent: {entity.Parent}, Version: {entity.Version})");


		protected bool Remove<T>(string id, bool refresh)
			where T : class, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id})");

		protected bool Remove<T>(string id, int version, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Version(version.HasNotNullArg("version"))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id}, Version: {version})");

		protected bool Remove<T, TParent>(string id, string parent, bool refresh)
			where T : BaseEntityWithParent<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection, IJoinProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(parent.HasNotNullArg(nameof(parent)))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id})");

		protected bool Remove<T, TParent>(string id, string parent, int version, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection, IJoinProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(parent.HasNotNullArg(nameof(parent)))
					.Version(version.HasNotNullArg(nameof(version)))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id}, Version: {version})");

		protected bool Remove<T, TParent>(string id, string parent, int version, Func<QueryContainerDescriptor<T>, QueryContainer> checkQuery, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection, IJoinProjection
			=> FilterCount<T>(q => q.Ids(i=>i.Values(id)) && q.ParentId(p=>p.Id(parent)) && checkQuery(q))>0 && Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(parent.HasNotNullArg(nameof(parent)))
					.Version(version.HasNotNullArg(nameof(version)))
					.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id}, Version: {version})");

		protected int Remove<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, bool refresh) where T : class, IRemoveProjection
			=> Try(
				c => c.DeleteByQuery<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(_mapping.ForTests || refresh, a => a.Refresh())),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);

		protected Task<int> RemoveAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, bool refresh) where T : class, IRemoveProjection
			=> TryAsync(
				c => c.DeleteByQueryAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(_mapping.ForTests || refresh, a => a.Refresh())),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);
	}
}
