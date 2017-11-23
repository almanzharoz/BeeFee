using Core.ElasticSearch;
using Core.ElasticSearch.Domain;

namespace WebApplication3.Models
{
	public abstract class PagerFilter
	{
		public int Page { get; set; }
		public int Size { get; set; }

		/// <param name="size">Значение размера страницы по-умолчанию</param>
		protected PagerFilter(int size) => Size = size;
	}

	public abstract class PagerFilter<T> : PagerFilter where T : class
	{
		public Pager<T> Items { get; private set; }

		protected void LoadItems(Pager<T> items) => Items = items;

		protected PagerFilter(int size) : base(size)
		{
		}
	}
}