﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Event;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class EventEditModel
	{
		public string Id { get; set; }
		public int Version { get; set; }

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		public string Url { get; set; }
		public string Cover { get; set; }

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

		public EEventType Type { get; set; }

		//[DataType(DataType.Currency)]
		//public string Price { get; set; }

		public string Html { get; set; }

		public IList<SelectListItem> Categories { get; private set; }

		public EventEditModel() { } // For binder

		public EventEditModel(EventProjection @event, IReadOnlyCollection<CategoryProjection> categories)
		{
			Init(categories);
			Id = @event.Id;
			Version = @event.Version;
			Name = @event.Name;
			Url = @event.Url;
			Type = @event.Type;
			CategoryId = @event.Category.Id;
			StartDateTime = @event.DateTime.Start;
			FinishDateTime = @event.DateTime.Finish;
			City = @event.Address.City;
			Address = @event.Address.AddressString;
			Html= @event.Page.Html;
			Cover = @event.Page.Cover;
		}

		public EventEditModel Init(IReadOnlyCollection<CategoryProjection> categories)
		{
			Categories = categories.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id }).ToList();
			return this;
		}
	}
}