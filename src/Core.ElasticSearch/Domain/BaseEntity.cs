using Newtonsoft.Json;

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

		protected BaseNewEntity() { }

		/// <summary>
		/// Используется, если хотим использовать свой Id при вставке
		/// </summary>
		/// <param name="id"></param>
		protected BaseNewEntity(string id)
		{
			Id = id;
		}
	}

}