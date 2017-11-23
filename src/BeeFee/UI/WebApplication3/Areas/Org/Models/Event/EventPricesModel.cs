
namespace WebApplication3.Areas.Org.Models.Event
{
	public class EventPricesModel : IEventEditModel
	{
		public BeeFee.Model.Embed.TicketPrice[] Prices { get; }

		public EventPricesModel(BeeFee.Model.Embed.TicketPrice[] prices)
		{
			Prices = prices;
		}
	}
}