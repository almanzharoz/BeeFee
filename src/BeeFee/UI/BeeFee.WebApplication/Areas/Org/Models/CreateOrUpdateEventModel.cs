using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateOrUpdateEventModel
    {
        [Required(ErrorMessage = "Company is required")]
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Label is required")]
        public string Label { get; set; }

        [RegularExpression(@"[a-zA-Z-_\d]{3,}")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Category is required")]
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

        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; }

        public string Html { get; set; }

        //[Required(ErrorMessage = "Cover is required")]
        public string Cover { get; set; }

        public string CompanyUrl { get; set; }

        //[DataType(DataType.Currency)]
        //public string Price { get; set; }

        public IList<SelectListItem> Categories { get; private set; }

		public int Step { get; set; }

		public IEventPageProjection Preview { get; private set; }

		public CreateOrUpdateEventModel Init<T>(T company, IReadOnlyCollection<BaseCategoryProjection> categories) where T : IProjection<Company>, IWithUrl
        {
            Categories = categories.Select(x => new SelectListItem {Text = x.Name, Value = x.Id, Selected = x.Id == CategoryId}).ToList();
			CompanyId = company.Id;
			CompanyUrl = company.Url;
            return this;
        }

		public void SetPreviewWithStep(IEventPageProjection @event)
		{
			Step = 4;
			Preview = @event.HasNotNullEntity("Event");
		}
		public void SetPreviewWithoutStep(IEventPageProjection @event)
		{
			Preview = @event.HasNotNullEntity("Event");
		}
    }
}
