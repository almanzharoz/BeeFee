using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public abstract class BaseEntityWithParent<T> : BaseEntity, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; internal set; }

		protected BaseEntityWithParent(string id, T parent) : base(id)
		{
			Parent = parent;
		}
	}

	public abstract class BaseEntityWithParentAndVersion<T> : BaseEntityWithVersion, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; internal set; }

		protected BaseEntityWithParentAndVersion(string id, T parent) : base(id)
		{
			Parent = parent;
		}

		protected BaseEntityWithParentAndVersion(string id, T parent, int version) : base(id, version)
		{
			Parent = parent;
		}
	}

	public abstract class BaseNewEntityWithParent<T> : BaseNewEntity, IWithParent<T>, IProjection
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; }

		protected BaseNewEntityWithParent(T parent)
		{
			Parent = parent;
		}

		protected BaseNewEntityWithParent(string id, T parent) : base(id)
		{
			Parent = parent;
		}
	}
}