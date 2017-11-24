using System;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Exceptions;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{

		#region GetById

		protected T GetById<T>(string id, bool load = true)
			where T : BaseEntity, IProjection, IGetProjection
			=> id.IsNull() ? null : _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(id)
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T GetWithVersionById<T>(string id, bool load = true)
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> id.IsNull() ? null : _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(id)
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields).VersionType(VersionType.Force)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected Task<T> GetWithVersionByIdAsync<T>(string id, bool load = true)
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> id.IsNull() ? null : _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.GetAsync(
							DocumentPath<T>.Id(id)
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields).VersionType(VersionType.Force)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T GetById<T>(string id, int version, bool load = true)
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(id.HasNotNullArg(nameof(id)))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Version(version.HasNotNullArg(nameof(version))).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Version: {version})"));

		protected Task<T> GetByIdAsync<T>(string id, int version, bool load = true)
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
			.Convert(
				projection => TryAsync(
					c => c.GetAsync(
					DocumentPath<T>.Id(id.HasNotNullArg(nameof(id)))
						.Index(projection.MappingItem.IndexName)
						.Type(projection.MappingItem.TypeName),
					x => x.Version(version.HasNotNullArg(nameof(version))).SourceInclude(projection.Fields)),
				r => r.Source.If(load, Load),
				RepositoryLoggingEvents.ES_GET,
				$"Get (Id: {id}, Version: {version})"));

		protected Task<T> GetByIdAsync<T>(string id, bool load = true)
			where T : class, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.GetAsync(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source // загруженный объект
							.IfAsync(load, LoadAsync), // загрузка полей
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		#endregion

		#region GetById WithParent

		protected T GetById<T, TParent>(string id, string parent, bool load = true)
			where T : BaseEntityWithParent<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent.HasNotNullArg(nameof(parent))).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent})"));

		protected T GetWithVersionById<T, TParent>(string id, string parent, bool load = true)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent.HasNotNullArg(nameof(parent))).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent})"));

		protected T GetById<T, TParent>(string id, string parent, int version, bool load = true)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent.HasNotNullArg(nameof(parent)))
								.Version(version.HasNotNullArg(nameof(version)))
								.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent}, Version: {version})"));

		#endregion

		#region GetByIdAndQuery

		protected TProjection GetByIdAndQuery<T, TProjection>(string id,
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntity, IProjection<T>, IGetProjection
			where T : class, IModel
			=> id.IsNull()
				? null
				: _mapping.GetProjectionItem<TProjection>()
					.Convert(
						projection => Try(
							c => c.Search<T, TProjection>(x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id)) &&
																query(new QueryContainerDescriptor<T>()))))
								.Take(1)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
							r => r.Documents.FirstOrDefault().If(x => load && x != null, x => Load()),
							RepositoryLoggingEvents.ES_GET,
							$"Get with query (Id: {id})"));

		protected TProjection GetByIdAndQuery<TProjection>(string id, int version,
			Func<QueryContainerDescriptor<TProjection>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntityWithVersion, IProjection, IGetProjection
			=> id.IsNull()
				? null
				: _mapping.GetProjectionItem<TProjection>()
					.Convert(
						projection => Try(
							c => c.Search<TProjection>(x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Query(q => q.Bool(b => b.Filter(Query<TProjection>.Ids(i => i.Values(id)) &&
																query(new QueryContainerDescriptor<TProjection>()))))
								.Version()
								.Take(1)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
							r => r.Documents.FirstOrDefault()
								.ThrowIf<TProjection, VersionException>(x => x.Version != version)
								.If(x => load && x != null, x => Load()),
							RepositoryLoggingEvents.ES_GET,
							$"Get with query (Id: {id})"));

		protected TProjection GetByIdAndQuery<T, TProjection>(string id, int version,
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntityWithVersion, IProjection<T>, IGetProjection
			where T : class, IModel
			=> id.IsNull()
				? null
				: _mapping.GetProjectionItem<TProjection>()
					.Convert(
						projection => Try(
							c => c.Search<T, TProjection>(x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id)) &&
																query(new QueryContainerDescriptor<T>()))))
								.Version()
								.Take(1)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
							r => r.Documents.FirstOrDefault()
								.ThrowIf<TProjection, VersionException>(x => x.Version != version)
								.If(x => load && x != null, x => Load()),
							RepositoryLoggingEvents.ES_GET,
							$"Get with query (Id: {id})"));

		protected TProjection GetWithVersionByIdAndQuery<T, TProjection>(string id,
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntityWithVersion, IProjection<T>, IGetProjection
			where T : class, IModel
			=> id.IsNull() ? null : _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id)) &&
															 query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id})"));

		protected Task<TProjection> GetWithVersionByIdAndQueryAsync<T, TProjection>(string id,
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntityWithVersion, IProjection<T>, IGetProjection
			where T : class, IModel
			=> id.IsNull() ? null : _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<T, TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id)) &&
															query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id})"));
		#endregion

		#region GetByIdAndQuery WithParent

		protected TProjection GetWithVersionByIdAndQuery<TProjection, TParent>(string id, string parent,
			Func<QueryContainerDescriptor<TProjection>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<TProjection>.Ids(i => i.Values(id.HasNotNullArg(nameof(id)))) &&
															Query<TProjection>.ParentId(p => p.Id(parent.HasNotNullArg(nameof(parent)))) &&
															query(new QueryContainerDescriptor<TProjection>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GETWITHQUERY,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected TProjection GetByIdAndQuery<T, TProjection, TParent>(string id, string parent, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParent<TParent>, IProjection<T>, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GETWITHQUERY,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected Task<TProjection> GetByIdAndQueryAsync<T, TProjection, TParent>(string id, string parent, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParent<TParent>, IProjection<T>, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GETWITHQUERY,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected TProjection GetByIdAndQuery<T, TProjection, TParent>(string id, string parent, int version, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection<T>, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
			.Convert(
				projection => Try(
				c => c.Search<TProjection>(x => x
					.Index(projection.MappingItem.IndexName)
					.Type(projection.MappingItem.TypeName)
					.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
													Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
					.Take(1)
					.Version()
					.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
				r => r.Documents.FirstOrDefault()
					.ThrowIf<TProjection, VersionException>(x => x.Version != version)
					.If(x => load && x != null, x => Load()),
				RepositoryLoggingEvents.ES_GETWITHQUERY,
				$"Get with query (Id: {id}, Parent: {parent})"));

		protected TProjection GetByIdAndQuery<TProjection, TParent>(string id, string parent, int version,
			Func<QueryContainerDescriptor<TProjection>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<TProjection>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<TProjection>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) &&
															query(new QueryContainerDescriptor<TProjection>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault()
							.ThrowIf<TProjection, VersionException>(x => x.Version != version)
							.If(x => load && x != null, x => Load()),
						RepositoryLoggingEvents.ES_GETWITHQUERY,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected Task<TProjection> GetByIdAndQueryAsync<TProjection, TParent>(string id, string parent, int version,
			Func<QueryContainerDescriptor<TProjection>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<TProjection>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<TProjection>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) &&
															query(new QueryContainerDescriptor<TProjection>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault()
							.ThrowIf<TProjection, VersionException>(x => x.Version != version)
							.If(x => load && x != null, x => Load()),
						RepositoryLoggingEvents.ES_GETWITHQUERY,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected TProjection GetWithVersionByIdAndQuery<T, TProjection, TParent>(string id, string parent, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection<T>, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected Task<TProjection> GetWithVersionByIdAndQueryAsync<T, TProjection, TParent>(string id, string parent, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where TProjection : BaseEntityWithParentAndVersion<TParent>, IProjection<T>, IGetProjection
			where TParent : class, IProjection, IJoinProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id}, Parent: {parent})"));
		#endregion

	}
}