using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{

		#region Update Entity

		protected bool Update<T>(T entity, bool refresh) where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(entity) //TODO: Было бы круто апдейтить только set - поля
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity.Id})");

		protected bool UpdateWithVersion<T>(T entity, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
							.Index(_mapping.GetIndexName<T>())
							.Type(_mapping.GetTypeName<T>())
							.Version(entity.Version.HasNotNullArg(nameof(entity.Version)))
							.Doc(entity)
							.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int) r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id}, Version: {entity?.Version})");

		protected bool Update<T>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter(entity))
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected Task<bool> UpdateAsync<T>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IUpdateProjection
			=> TryAsync(c => c.UpdateAsync(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter(entity))
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool UpdateWithVersion<T>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter(entity))
						.Version(entity.Version)
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool UpdateWithVersion<T, TParent>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			=> Try(c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Parent(entity.Parent.HasNotNullArg(x => x.Id, nameof(entity.Parent)).Id)
						.Version(entity.Version.HasNotNullArg("version"))
						.Doc(setter(entity))
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id}), Version: {entity?.Version}");

		protected Task<bool> UpdateWithVersionAsync<T, TParent>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			=> TryAsync(c => c.UpdateAsync(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Parent(entity.Parent.HasNotNullArg(x => x.Id, nameof(entity.Parent)).Id)
						.Version(entity.Version.HasNotNullArg("version"))
						.Doc(setter(entity))
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id}), Version: {entity?.Version}");

		#endregion

		#region UpdateById

		public bool UpdateById<T>(string id, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IGetProjection, IUpdateProjection
			=> GetById<T>(id).HasNotNullArg("entity")
				.Convert(entity =>
					Try(c => c.Update(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(setter(entity))
								.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id})"));

		public Task<bool> UpdateByIdAsync<T>(string id, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IGetProjection, IUpdateProjection
			=> GetById<T>(id).HasNotNullArg("entity")
				.Convert(entity =>
					TryAsync(c => c.UpdateAsync(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(setter(entity))
								.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id})"));

		protected bool UpdateById<T>(string id, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IGetProjection, IUpdateProjection
			=> GetById<T>(id, version).HasNotNullArg("entity")
				.Convert(entity =>
					Try(c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected Task<bool> UpdateByIdAsync<T>(string id, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IGetProjection, IUpdateProjection
			=> GetById<T>(id, version).HasNotNullArg("entity")
				.Convert(entity =>
					TryAsync(c => c.UpdateAsync(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected bool UpdateById<T, TParent>(string id, string parent, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			=> GetById<T, TParent>(id, parent, version)
				.Convert(entity =>
					Try(c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Parent(parent.HasNotNullArg(nameof(parent)))
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int) r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected Task<bool> UpdateByIdAsync<T, TParent>(string id, string parent, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			=> GetById<T, TParent>(id, parent, version)
				.Convert(entity =>
					TryAsync(c => c.UpdateAsync(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Parent(parent.HasNotNullArg(nameof(parent)))
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected bool UpdateById<T>(string id, Func<T> setter, bool refresh)
			where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(c => c.Update(
					DocumentPath<T>.Id(id.HasNotNullArg("id")), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter())
						.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {id})");

		#endregion

		#region UpdateByIdAndQuery

		/// <summary>
		/// Хитрожопая функция обновления. Пытается взять документ по id и query, если такой не найден кидает эксепшн.
		/// Иначе обновляет достанный документ указанной функцией обновления.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TAccessException"></typeparam>
		/// <param name="id">Id документа</param>
		/// <param name="version">Версия документа</param>
		/// <param name="query">Запрос для проверки на доступность</param>
		/// <param name="getException">Генерация ошибки, если документ не найден</param>
		/// <param name="setter">Функция обновления</param>
		/// <param name="refresh">Флаг обновления индекса сразу</param>
		/// <returns>Документ изменен. Если документ не изменился, хоть и был найден, вернется false.</returns>
		protected bool UpdateByIdAndQuery<T, TAccessException>(string id, int version,
			Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<TAccessException> getException, Func<T, T> setter,
			bool refresh)
			where T : BaseEntityWithVersion, IProjection, IGetProjection, IUpdateProjection
			where TAccessException : Exception
			=> GetByIdAndQuery(id, version, query, false)
				.ThrowIfNull(getException)
				.Convert(entity =>
					Try(c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int) r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected Task<bool> UpdateByIdAndQueryAsync<T, TAccessException>(string id, int version,
			Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<TAccessException> getException, Func<T, T> setter,
			bool refresh)
			where T : BaseEntityWithVersion, IProjection, IGetProjection, IUpdateProjection
			where TAccessException : Exception
			=> GetByIdAndQuery(id, version, query, false)
				.ThrowIfNull(getException)
				.Convert(entity =>
					TryAsync(c => c.UpdateAsync(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected bool UpdateByIdAndQuery<T, TParent, TAccessException>(string id, string parent, int version,
			Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<TAccessException> getException, Func<T, T> setter,
			bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			where TAccessException : Exception
			=> GetByIdAndQuery<T, TParent>(id, parent, version, query, false)
				.ThrowIfNull(getException)
				.Convert(entity =>
					Try(c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int) r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		#endregion

		#region UpdateByQuery

		// TODO: Добавить обновление с TPartial
		protected bool UpdateWithFilter<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort, Func<T, T> update, bool refresh)
			where T : BaseEntity, ISearchProjection, IProjection, IUpdateProjection
			=> Filter<T>(query, sort, 0, 1)
				.FirstOrDefault()
				.NotNullOrDefault(entity =>
					Try(c => c.Update(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(update(entity))
								.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {entity.Id})"));

		protected async Task<bool> UpdateWithFilterAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort, Func<T, T> update, bool refresh)
			where T : BaseEntity, ISearchProjection, IProjection, IUpdateProjection
			=> await (await FilterFirstAsync<T>(query, sort))
				.NotNullOrDefault(entity =>
					TryAsync(c => c.UpdateAsync(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(update(entity))
								.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {entity.Id})"));

		protected T UpdateWithFilterAndVersion<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort, Func<T, T> update, bool refresh)
			where T : BaseEntityWithVersion, ISearchProjection, IProjection, IUpdateProjection
			=> Filter<T>(query, sort, 0, 1)
				.FirstOrDefault()
				.IfNotNull(entity =>
					Try(c => c.Update(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(update(entity))
								.If(_mapping.ForTests || refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated ? entity.Set(x => x.Version, entity.Version + 1) : null,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {entity.Id})"));


		protected int Update<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update, bool refresh)
			where T : class, IProjection, IUpdateProjection
			=> Try(c => c.UpdateByQuery<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					//.Version()
					.If(_mapping.ForTests || refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => (int) r.Updated,
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected Task<int> UpdateAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update, bool refresh)
			where T : class, IProjection, IUpdateProjection
			=> TryAsync(c => c.UpdateByQueryAsync<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					//.Version()
					.If(_mapping.ForTests || refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => (int)r.Updated,
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		//protected Task<(int updated, int total)> UpdateAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
		//	Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update,
		//	bool refresh)
		//	where T : class, IProjection, IUpdateProjection
		//	=> TryAsync(c => c.UpdateByQueryAsync<T>(x => x
		//			.Index(_mapping.GetIndexName<T>())
		//			.Type(_mapping.GetTypeName<T>())
		//			.Query(q => q.Bool(b => b.Filter(query)))
		//			.Version()
		//			.If(_mapping.ForTests || refresh, y => y.Refresh())
		//			.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
		//		r => ((int) r.Updated, (int) r.Total),
		//		RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		#endregion
	}
}