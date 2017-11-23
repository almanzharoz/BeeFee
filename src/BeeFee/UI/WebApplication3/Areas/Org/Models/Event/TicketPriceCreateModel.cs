using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Areas.Org.Models.Event
{
	public class TicketPriceCreateModel
	{
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		public string Template { get; set; }
		public decimal Price { get; set; }
		[Required]
		public int Count { get; set; }
	}
}