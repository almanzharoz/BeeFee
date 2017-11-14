using System.ComponentModel.DataAnnotations;
using BeeFee.ClientApp.Projections.Event;

namespace WebApplication3.Models.Event
{
	public class EventPageModel
	{
		public EventPageModel()
		{
		}

		public EventPageModel(EventProjection @event, string name, string email, string phone)
		{
			Event = @event;
			Name = name;
			Email = email;
			Phone = phone;
		}

		public EventProjection Event { get; }

		[Required]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[Phone]
		public string Phone { get; set; }
	}
}