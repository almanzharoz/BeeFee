namespace Core.ElasticSearch.Domain
{
	public interface IWithVersion
	{
		int Version { get; }
	}
}