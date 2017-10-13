using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.Model.Projections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateOrUpdateEventInfoModel
    {
        public CreateOrUpdateEventInfoModel()
        {
        }

        public CreateOrUpdateEventInfoModel(CreateOrUpdateEventModel model)
        {
            this.CompanyId = model.CompanyId;
            this.Categories = model.Categories;
            this.Address = model.Address;
            this.Email = model.Email;
            this.City = model.City;
            this.Address = model.Address;
            this.CompanyUrl = model.CompanyUrl;
            this.Cover = model.Cover;
            this.File = model.File;
            this.StartDateTime = model.StartDateTime;
            this.FinishDateTime = model.FinishDateTime;
            this.Label = model.Label;
            this.Name = model.Name;
            this.Url = model.Url;
        }

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


        //[Required(ErrorMessage = "Cover is required")]
        public string Cover { get; set; }

        public IFormFile File { get; set; }

        public string CompanyUrl { get; set; }

        //[DataType(DataType.Currency)]
        //public string Price { get; set; }

        public IList<SelectListItem> Categories { get; private set; }

        public CreateOrUpdateEventInfoModel Init(IReadOnlyCollection<BaseCategoryProjection> categories)
        {
            Categories = categories.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id }).ToList();
            return this;
        }
    }
}
