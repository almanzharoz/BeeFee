using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Projections.Event;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class UpdateEventModel : CreateOrUpdateEventModel
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Version is required")]
        public int Version { get; set; }

        public string[] Comments { get; private set; }

        public UpdateEventModel() { } // For binder

        public UpdateEventModel(EventProjection @event, IReadOnlyCollection<BaseCategoryProjection> categories)
        {
            Id = @event.Id;
            Version = @event.Version;
            Name = @event.Name;
            Label = @event.Page.Label;
            Url = @event.Url;
            CategoryId = @event.Category.Id;
            StartDateTime = @event.DateTime.Start;
            FinishDateTime = @event.DateTime.Finish;
            City = @event.Address.City;
            Address = @event.Address.AddressString;
            Html = @event.Page.Html;
            Cover = @event.Page.Cover;
            Email = @event.Email;
            Comments = @event.Comments;
			Init(@event.Parent, categories);
        }

		public UpdateEventModel Saved()
		{
			Version++;
			return this;
		}
	}
}