namespace WebApplication3.Models
{
	public abstract class PagerFilter
	{
		public int Page { get; set; }
		public int Size { get; set; }

		protected PagerFilter(int size) => Size = size;
	}
}