﻿using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected bool Insert<T>(T entity, bool refresh) where T : BaseNewEntity, IProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool InsertWithId<T>(T entity, bool refresh) where T : BaseNewEntityWithId, IProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.OpType(OpType.Create)
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh) where T : BaseNewEntity, IProjection
			=> TryAsync(
				c => c.IndexAsync(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.IfNotNull(entity.Id, x => x.OpType(OpType.Create))
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected Task<bool> InsertWithIdAsync<T>(T entity, bool refresh) where T : BaseNewEntityWithId, IProjection
			=> TryAsync(
				c => c.IndexAsync(entity.HasNotNullArg(nameof(entity)), s => s
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.OpType(OpType.Create)
					.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T, TParent>(T entity, bool refresh)
			where T : BaseNewEntityWithParent<TParent>
			where TParent : IProjection, IJoinProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Parent(entity.Parent.Id)
						.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected Task<bool> InsertAsync<T, TParent>(T entity, bool refresh)
			where T : BaseNewEntityWithParent<TParent>
			where TParent : IProjection, IJoinProjection
			=> TryAsync(
				c => c.IndexAsync(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Parent(entity.Parent.Id)
						.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True)))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool InsertWithId<T, TParent>(T entity, bool refresh)
			where T : BaseNewEntityWithIdAndParent<TParent>
			where TParent : IProjection, IJoinProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.OpType(OpType.Create)
						.Parent(entity.Parent.Id)
						.If(_mapping.ForTests || refresh, a => a.Refresh(Refresh.True))),
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
				r => r.Created ? GetById<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

		/// <summary>
		/// Для тестов.
		/// </summary>
		/// <typeparam name="TNew"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected T InsertWithId<TNew, T>(TNew entity) where TNew : BaseNewEntityWithId, IProjection
			where T : BaseEntity, IProjection, IGetProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.OpType(OpType.Create)
						.Refresh(Refresh.True)),
				r => r.Created ? GetById<T>(entity.Id) : null,
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
						.Refresh(Refresh.True))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created ? GetWithVersionById<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

		protected Task<T> InsertWithVersionAsync<TNew, T>(TNew entity) where TNew : BaseNewEntity, IProjection
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> TryAsync(
				c => c.IndexAsync(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Refresh(Refresh.True))
					.Fluent(x => entity.Id = x.Id),
				r => r.Created ? GetWithVersionById<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

		/// <summary>
		/// Для тестов.
		/// </summary>
		/// <typeparam name="TNew"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected T InsertWithIdAndVersion<TNew, T>(TNew entity) where TNew : BaseNewEntityWithId, IProjection
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> Try(
				c => c.Index(entity.HasNotNullArg(nameof(entity)), s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.OpType(OpType.Create)
						.Refresh(Refresh.True)),
				r => r.Created ? GetWithVersionById<T>(entity.Id) : null,
				RepositoryLoggingEvents.ES_INSERT);

	}
}