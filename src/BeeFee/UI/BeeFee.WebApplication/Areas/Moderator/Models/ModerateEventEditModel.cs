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

		[Required]
		public string CategoryId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Caption { get; set; }
		[Required]
		public string Html { get; set; }
		[Required]
		public Address Address { get; set; }
		[Required]
		public string Label { get; set; }

		public ModerateEventEditModel() { }
		public ModerateEventEditModel(EventModeratorProjection @event)
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
		}
	}
}
