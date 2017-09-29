using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
	public class AddEventEditModel
	{
		[Required(ErrorMessage = "Company is required")]
		public string CompanyId { get; set; }

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Label is required")]
		public string Label { get; set; }

		[RegularExpression(@"[a-zA-Z-_]{3,}")]
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
		[EmailAddress]
		public string Email { get; set; }

		//[DataType(DataType.Currency)]
		//public string Price { get; set; }

		public string Html { get; set; }

		public IList<SelectListItem> Categories { get; private set; }

		public AddEventEditModel() { } // For binder

		public AddEventEditModel(CompanyProjection company, IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			CompanyId = company.Id;
			Email = company.Email;
			Init(categories);
		}

		public AddEventEditModel Init(IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			Categories = categories.Select(x => new SelectListItem(){Text = x.Name, Value = x.Id}).ToList();
			return this;
		}
	}
}