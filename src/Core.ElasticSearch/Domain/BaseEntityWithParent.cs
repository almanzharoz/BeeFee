using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Domain
{
	//TODO: Сделать для IJoinProjection
	public abstract class BaseEntityWithParent<T> : IEntity, IWithParent<T>
		where T : IProjection, IJoinProjection
	{
		[JsonIgnore]
		public string Id { get; }
		[JsonIgnore]
		public T Parent { get; internal set; }

		protected BaseEntityWithParent(string id) // for IJoinProjection
		{
			Id = id;
		}

		protected BaseEntityWithParent(string id, T parent) : this(id)
		{
			Parent = parent;
		}
	}

	public abstract class BaseEntityWithParentAndVersion<T> : IEntity, IWithParent<T>, IWithVersion
		where T : IProjection, IJoinProjection
	{
		[JsonIgnore]
		public string Id { get; }
		[JsonIgnore]
		public int Version { get; internal set; }
		[JsonIgnore]
		public T Parent { get; internal set; }

		protected BaseEntityWithParentAndVersion(string id)
		{
			Id = id;
		}
		protected BaseEntityWithParentAndVersion(string id, T parent) : this(id)
		{
			Parent = parent;
		}

		protected BaseEntityWithParentAndVersion(string id, T parent, int version) : this(id)
		{
			Version = version;
			Parent = parent;
		}
	}

	public abstract class BaseNewEntityWithParent<T> : IWithParent<T>, IProjection
		where T : IProjection, IJoinProjection
	{
		[JsonIgnore]
		public string Id { get; internal set; }
		[JsonIgnore]
		public T Parent { get; }

		protected BaseNewEntityWithParent(T parent)
		{
			Parent = parent.HasNotNullArg(x => x.Id, nameof(parent));
		}
	}

	public abstract class BaseNewEntityWithIdAndParent<T> : IWithParent<T>, IProjection
		where T : IProjection, IJoinProjection
	{
		[JsonIgnore]
		public string Id { get; }
		[JsonIgnore]
		public T Parent { get; }

		protected BaseNewEntityWithIdAndParent(string id, T parent)
		{
			Id = id.HasNotNullArg(nameof(id));
			Parent = parent.HasNotNullArg(x => x.Id, nameof(parent));
		}
	}
}