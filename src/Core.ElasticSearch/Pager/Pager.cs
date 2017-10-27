using System.Collections;
using System.Collections.Generic;
using Core.ElasticSearch.Domain;

namespace Core.ElasticSearch
{
    public class Pager<T> : IPager, IReadOnlyCollection<T>
		where T : IProjection
    {
	    private readonly IReadOnlyCollection<T> _items;
		public int Page { get; }
		public int Limit { get; }
	    public int Count => _items.Count;
	    public int Total { get; }
	    public int Pages { get; }

		public Pager(int page, int limit, int total, IReadOnlyCollection<T> items)
		{
			Page = page < 1 ? 1 : page; // 0 и 1 - всегда первая страница
			Pages = total / limit + (total % limit > 0 ? 1 : 0);
		    Limit = limit;
		    Total = total;
		    _items = items;
	    }

	    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

	    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
