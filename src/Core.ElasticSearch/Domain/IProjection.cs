
namespace Core.ElasticSearch.Domain
{
	public interface IProjection : IEntity
	{
		
	}

	/// <summary>
	/// Использование проекции для обновления
	/// </summary>
	public interface IUpdateProjection : IEntity
	{

	}
	/// <summary>
	/// Использование проекции для связей
	/// </summary>
	public interface IJoinProjection : IEntity
	{

	}
	/// <summary>
	/// Использование проекции для поиска по Id
	/// </summary>
	public interface IGetProjection : IEntity
	{

	}
	/// <summary>
	/// Использование проекции для поиска
	/// </summary>
	public interface ISearchProjection : IEntity
	{

	}
	/// <summary>
	/// Использование проекции для удаления
	/// </summary>
	public interface IRemoveProjection : IEntity
	{

	}


	public interface IProjection<T> : IProjection where T : class, IModel
	{
		
	}
}