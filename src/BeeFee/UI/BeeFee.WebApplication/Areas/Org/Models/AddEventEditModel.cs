using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class AddEventEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Label is required")]
		public string Label { get; set; }

		public string Url { get; set; }

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

		[Required(ErrorMessage = "Cover is required")]
		public string Cover { get; set; }

		public EEventType Type { get; set; }

		//[DataType(DataType.Currency)]
		//public string Price { get; set; }

		public string Html { get; set; }

		public IList<SelectListItem> Categories { get; private set; }

		public AddEventEditModel() { } // For binder

		public AddEventEditModel(IReadOnlyCollection<CategoryProjection> categories)
			=> Init(categories);

		public AddEventEditModel Init(IReadOnlyCollection<CategoryProjection> categories)
		{
			Categories = categories.Select(x => new SelectListItem(){Text = x.Name, Value = x.Id}).ToList();
			return this;
		}
	}
}