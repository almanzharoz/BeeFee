using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Areas.Org.Models.TicketPrice
{
	public class TicketPriceEditModel
	{
		public string Template { get; set; }
		public decimal Price { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Required]
		public int Count { get; set; }

		public TicketPriceEditModel() { }

		public TicketPriceEditModel(BeeFee.Model.Embed.TicketPrice ticket)
		{
			Template = ticket.TicketTemplate;
			Price = ticket.Price;
			Name = ticket.Name;
			Description = ticket.Description;
			Count = ticket.Count;
		}
	}
}