using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Event;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class EventEditModel
	{
		[Required(ErrorMessage = "Id is required")]
		public string Id { get; set; }
		[Required(ErrorMessage = "Company is required")]
		public string CompanyId { get; set; }
		public string CompanyUrl { get; set; }
		[Required(ErrorMessage = "Version is required")]
		public int Version { get; set; }

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Label is required")]
		public string Label { get; set; }

		[RegularExpression(@"[a-zA-Z-_\d]{3,}")]
		public string Url { get; set; }
		[Required(ErrorMessage = "Cover is required")]
		public string Cover { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		public string CategoryId { get; set; }

		[Required(ErrorMessage = "Start date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime StartDateTime { get; set; }

		[Required(ErrorMessage = "Finish date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FinishDateTime { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }

		//[DataType(DataType.Currency)]
		//public string Price { get; set; }

		public string Html { get; set; }

		public string[] Comments { get; private set; }


		public IList<SelectListItem> Categories { get; private set; }

		public EventEditModel() { } // For binder

		public EventEditModel(EventProjection @event, IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			Init(categories);
			Id = @event.Id;
			CompanyId = @event.Parent.Id;
			CompanyUrl = @event.Parent.Url;
			Version = @event.Version;
			Name = @event.Name;
			Label = @event.Page.Label;
			Url = @event.Url;
			CategoryId = @event.Category.Id;
			StartDateTime = @event.DateTime.Start;
			FinishDateTime = @event.DateTime.Finish;
			City = @event.Address.City;
			Address = @event.Address.AddressString;
			Html= @event.Page.Html;
			Cover = @event.Page.Cover;
			Email = @event.Email;
			Comments = @event.Comments;
		}

		public EventEditModel Init(IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			Categories = categories.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id }).ToList();
			return this;
		}
	}
}