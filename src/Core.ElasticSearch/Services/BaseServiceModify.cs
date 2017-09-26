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
		protected bool Update<T>(T entity, bool refresh) where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(entity) //TODO: Было бы круто апдейтить только set - поля
						.If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity.Id})");

		protected bool UpdateWithVersion<T>(T entity, bool refresh) where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity)).Id), d => d
							.Index(_mapping.GetIndexName<T>())
							.Type(_mapping.GetTypeName<T>())
							.Version(entity.Version)
							.Doc(entity)
							.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int)r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id}, Version: {entity?.Version})");

		protected bool Update<T>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter(entity))
						.If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool UpdateWithVersion<T>(T entity, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Version(entity.Version.HasNotNullArg("version"))
						.Doc(setter(entity))
						.If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id}), Version: {entity?.Version}");

		protected bool Update<T>(string id, Func<T, T> setter, bool refresh)
			where T : BaseEntity, IProjection, IGetProjection, IUpdateProjection
			=> Get<T>(id)
				.Convert(entity =>
					Try(
						c => c.Update(
							DocumentPath<T>.Id(entity.Id), d => d
								.Index(_mapping.GetIndexName<T>())
								.Type(_mapping.GetTypeName<T>())
								.Doc(setter(entity))
								.If(refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id})"));

		protected bool Update<T>(string id, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithVersion, IProjection, IGetProjection, IUpdateProjection
			=> Get<T>(id, version)
				.Convert(entity =>
					Try(
						c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Doc(setter(entity))
									.If(refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		protected bool Update<T, TParent>(string id, string parent, int version, Func<T, T> setter, bool refresh)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection, IUpdateProjection
			where TParent : class, IProjection, IJoinProjection
			=> Get<T, TParent>(id, parent, version)
				.Convert(entity =>
					Try(
						c => c.Update(
								DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity)).Id), d => d
									.Index(_mapping.GetIndexName<T>())
									.Type(_mapping.GetTypeName<T>())
									.Version(entity.Version)
									.Parent(parent.HasNotNullArg(nameof(parent)))
									.Doc(setter(entity))
									.If(refresh, x => x.Refresh(Refresh.True)))
							.Fluent(r => entity.Version = (int)r.Version),
						r => r.Result == Result.Updated,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {id}, Version: {version})"));

		//protected bool Update<T>(string id, int version, Func<T, T> setter) where T : BaseEntityWithVersion, IGetProjection, IProjection, IUpdateProjection 
		//	=> Update(id, version, setter, true);

		//protected bool Update<T>(string id, Func<T, T> setter) where T : BaseEntity, IProjection, IGetProjection, IUpdateProjection 
		//	=> Update(id, setter, true);

		protected bool Update<T>(string id, Func<T> setter, bool refresh) where T : BaseEntity, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
					DocumentPath<T>.Id(id.HasNotNullArg("id")), d => d
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.Doc(setter())
						.If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {id})");

		//protected bool Update<T>(string id, Func<T> setter) where T : BaseEntity, IProjection, IUpdateProjection => Update(id, setter, true);

		//protected Task<bool> UpdateAsync<T>(T entity, bool refresh = true)
		//	where T : BaseEntityWithVersion, IProjection, IUpdateProjection, IWithVersion
		//	=> TryAsync(
		//		c => c.UpdateAsync(
		//				DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))), d => d
		//					.Index(_mapping.GetIndexName<T>())
		//					.Type(_mapping.GetTypeName<T>())
		//					.Version(entity.Version)
		//					.Doc(entity)
		//					.If(refresh, x => x.Refresh(Refresh.True)))
		//			.Fluent(r => entity.Version = (int) r.Version),
		//		r => r.Result == Result.Updated,
		//		RepositoryLoggingEvents.ES_UPDATE,
		//		$"Update (Id: {entity?.Id})");

		// TODO: Добавить обновление с TPartial
		protected T UpdateWithFilter<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
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
								.If(refresh, x => x.Refresh(Refresh.True))),
						r => r.Result == Result.Updated ? entity.Set(x => x.Version, entity.Version + 1) : null,
						RepositoryLoggingEvents.ES_UPDATE,
						$"Update (Id: {entity.Id})"));


		protected int Update<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update, bool refresh)
			where T : class, IProjection, IUpdateProjection
			=> Try(
				c => c.UpdateByQuery<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					//.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => (int)r.Updated,
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected Task<(int updated, int total)> UpdateAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update,
			bool refresh)
			where T : class, IProjection, IUpdateProjection
			=> TryAsync(
				c => c.UpdateByQueryAsync<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => ((int)r.Updated, (int)r.Total),
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);
	}
}