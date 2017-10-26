namespace Core.ElasticSearch
{
	public interface IPager
	{
		int Page { get; }
		int Limit { get; }
		int Count { get; }
		int Pages { get; }
		int Total { get; }

	}
}