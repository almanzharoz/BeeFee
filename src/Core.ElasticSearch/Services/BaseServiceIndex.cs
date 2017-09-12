﻿using System;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		//protected bool Insert<T>(T entity) where T : BaseNewEntity => Insert(entity, true);
		protected bool Insert<T>(T entity, bool refresh) where T : BaseNewEntity, IProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh) where T : BaseNewEntity, IProjection
			=> TryAsync(
				c => c.IndexAsync(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		/// <summary>
		/// Для тестов.
		/// </summary>
		/// <typeparam name="TNew"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected T Insert<TNew, T>(TNew entity) where TNew : BaseNewEntity, IProjection
			where T : BaseEntity, IProjection, IGetProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.Refresh(Refresh.True))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created ? Get<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

		/// <summary>
		/// Для тестов.
		/// </summary>
		/// <typeparam name="TNew"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected T InsertWithVersion<TNew, T>(TNew entity) where TNew : BaseNewEntity, IProjection
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.Refresh(Refresh.True))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created ? GetWithVersion<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

		//protected bool Insert<T, TParent>(T entity)
		//	where T : BaseNewEntityWithParent<TParent>
		//	where TParent : IProjection
		//	=> Insert<T, TParent>(entity, true);

		protected bool Insert<T, TParent>(T entity, bool refresh)
			where T : BaseNewEntityWithParent<TParent>
			where TParent : IProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Parent(entity.Parent.HasNotNullArg(x => x.Id, "parent").Id)
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.If(refresh, a => a.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		//protected bool InsertWithVersion<T, TParent>(T entity)
		//	where T : BaseNewEntityWithParentAndVersion<TParent>
		//	where TParent : IProjection
		//	=> InsertWithVersion<T, TParent>(entity, true);

		//protected Task<bool> InsertAsync<T>(T entity, bool refresh = true)
		//	where T : BaseEntity, IProjection, IInsertProjection
		//	=> TryAsync(
		//		c => c.IndexAsync(entity, s => s
		//				.Index(_mapping.GetIndexName<T>())
		//				.Type(_mapping.GetTypeName<T>())
		//				.If(refresh, x => x.Refresh(Refresh.True)))
		//			.Fluent(x => entity
		//				.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
		//				.Id = x.Id),
		//		r => r.Created,
		//		RepositoryLoggingEvents.ES_INSERT);

		protected bool Remove<T>(T entity, bool refresh)
			where T : BaseEntity, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(refresh, a => a.Refresh(Refresh.True))),
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
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Version: {entity.Version})");

		protected bool Remove<T, TParent>(T entity, bool refresh)
			where T : BaseEntityWithParent<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(entity.Parent.HasNotNullArg(p => p.Id, "parent").Id)
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Parent: {entity.Parent})");

		protected bool RemoveWithVersion<T, TParent>(T entity, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(entity.Parent.HasNotNullArg(p => p.Id, "parent").Id)
					.Version(entity.Version.HasNotNullArg("version"))
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id}, Parent: {entity.Parent}, Version: {entity.Version})");

		protected bool Remove<T>(string id, bool refresh)
			where T : class, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(refresh, a => a.Refresh(Refresh.True))),
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
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id}, Version: {version})");

		protected bool Remove<T, TParent>(string id, string parent, bool refresh)
			where T : BaseEntityWithParent<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(parent.HasNotNullArg(nameof(parent)))
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id})");

		protected bool Remove<T, TParent>(string id, string parent, int version, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IRemoveProjection
			where TParent : class, IProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(id.HasNotNullArg(nameof(id))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Parent(parent.HasNotNullArg(nameof(parent)))
					.Version(version.HasNotNullArg(nameof(version)))
					.If(refresh, a => a.Refresh(Refresh.True))),
				r => r.Result == Result.Deleted,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {id}, Version: {version})");

		protected int Remove<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, bool refresh) where T : class, IRemoveProjection
			=> Try(
				c => c.DeleteByQuery<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(refresh, a => a.Refresh())),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);

		protected Task<int> RemoveAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, bool refresh) where T : class, IRemoveProjection
			=> TryAsync(
				c => c.DeleteByQueryAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.If(refresh, a => a.Refresh())),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);
	}
}