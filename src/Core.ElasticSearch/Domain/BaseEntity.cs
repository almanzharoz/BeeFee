using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Domain
{
	public abstract class BaseEntity : IEntity
	{
		[JsonIgnore]
		public string Id { get; }

		protected BaseEntity(string id)
		{
			Id = id;
		}
	}

	public abstract class BaseEntityWithVersion : IEntity, IWithVersion
	{
		[JsonIgnore]
		public string Id { get; }

		[JsonIgnore]
		public int Version { get; internal set; }

		/// <summary>
		/// Для IJoinProjection
		/// </summary>
		/// <param name="id"></param>
		protected BaseEntityWithVersion(string id)
		{
			Id = id;
		}

		protected BaseEntityWithVersion(string id, int version)
		{
			Id = id;
			Version = version;
		}
	}

	/// <summary>
	/// Используется для добавления новых документов. Такие объекты никогда не попадают в RequestContainer.
	/// </summary>
	public abstract class BaseNewEntity
	{
		[JsonIgnore]
		public string Id { get; internal set; }
	}

	/// <summary>
	/// Используется для добавления новых документов c заданным Id. Такие объекты никогда не попадают в RequestContainer.
	/// </summary>
	public abstract class BaseNewEntityWithId
	{
		[JsonIgnore]
		public string Id { get; }

		/// <summary>
		/// Используется, если хотим использовать свой Id при вставке
		/// </summary>
		/// <param name="id"></param>
		protected BaseNewEntityWithId(string id)
		{
			Id = id.HasNotNullArg(nameof(id));
		}
	}

}