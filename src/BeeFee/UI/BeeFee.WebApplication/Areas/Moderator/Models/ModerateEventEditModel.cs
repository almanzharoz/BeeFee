using BeeFee.Model.Embed;
using BeeFee.ModeratorApp.Projections;
using System.ComponentModel.DataAnnotations;

namespace BeeFee.WebApplication.Areas.Moderator.Models
{
	public class ModerateEventEditModel
    {
		[Required]
		public string Id { get; set; }
		[Required]
		public string CompanyId { get; set; }
		public int Version { get; set; }

		public string CategoryId { get; private set; }
		public string Name { get; private set; }
		public string Title { get; private set; }
		public string Caption { get; private set; }
		public string Html { get; private set; }
		public Address Address { get; private set; }
		public string Label { get; private set; }

		public string Comment { get; set; }
		public bool Result { get; set; }

		public ModerateEventEditModel() { }
		public ModerateEventEditModel(EventModeratorProjection @event)
		{
			Init(@event);
		}

		public ModerateEventEditModel Init(EventModeratorProjection @event)
		{
			Id = @event.Id;
			CompanyId = @event.Parent.Id;
			Version = @event.Version;

			CategoryId = @event.Category.Id;
			Name = @event.Name;
			Title = @event.Page.Title;
			Caption = @event.Page.Caption;
			Html = @event.Page.Html;
			Label = @event.Page.Label;
			Address = @event.Page.Address;
			return this;
		}
	}
}
