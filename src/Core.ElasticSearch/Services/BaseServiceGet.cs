using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected T Get<T>(string id, bool load = true)
			where T : BaseEntity, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(id.HasNotNullArg(nameof(id)))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T GetWithVersion<T>(string id, bool load = true)
			where T : BaseEntityWithVersion, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(id.HasNotNullArg(nameof(id)))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields).VersionType(VersionType.Force)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T Get<T>(string id, int version, bool load = true)
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

		protected T Get<T, TParent>(string id, string parent, bool load = true)
			where T : BaseEntityWithParent<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent})"));

		protected T GetWithVersion<T, TParent>(string id, string parent, bool load = true)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent})"));

		protected T Get<T, TParent>(string id, string parent, int version, bool load = true)
			where T : BaseEntityWithParentAndVersion<TParent>, IProjection, IGetProjection
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent)
								.Version(version.HasNotNullArg(nameof(version)))
								.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id}, Parent: {parent}, Version: {version})"));

		protected TProjection Get<T, TProjection>(string id, Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntity, IProjection<T>, IGetProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id})"));

		protected TProjection GetWithVersion<T, TProjection>(string id,
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			bool load = true)
			where TProjection : BaseEntityWithVersion, IProjection<T>, IGetProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
							                                 query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.Version()
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id})"));

		protected T Get<T, TParent>(string id, string parent, Func<QueryContainerDescriptor<T>, QueryContainer> query, bool load = true)
			where T : class, IProjection, IGetProjection, IWithParent<TParent>
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Search<T>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query(new QueryContainerDescriptor<T>()))))
							.Take(1)
							.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(T)), y => y.Version())
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected Task<T> GetAsync<T>(string id, bool load = true)
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
	}
}