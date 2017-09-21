namespace Core.ElasticSearch.Domain
{
	public interface IWithParent<T>
		where T : IProjection, IJoinProjection
	{
		T Parent { get; }
	}
}