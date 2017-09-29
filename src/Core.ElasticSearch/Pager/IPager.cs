namespace Core.ElasticSearch
{
	public interface IPager
	{
		int Page { get; }
		int Limit { get; }
		int Count { get; }
		int Total { get; }

	}
}