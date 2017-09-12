namespace Core.ElasticSearch.Domain
{
	public interface IWithParent<T>
		where T : IProjection
	{
		T Parent { get; }
	}
}