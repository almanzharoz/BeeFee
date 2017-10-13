using System.ComponentModel.DataAnnotations;
using BeeFee.ClientApp.Projections.Event;

namespace BeeFee.WebApplication.Models.Event
{
    public class EventPageModel
    {
        public EventPageModel()
        {
        }

        public EventPageModel(EventProjection @event, string name, string email, string phone, int? registerResult)
        {
            this.Event = @event;
            this.Name = name;
            this.Email = email;
            this.Phone = phone;
            this.RegisterResult = registerResult;
        }

        public EventProjection Event { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public int? RegisterResult { get; set; }
    }
}
