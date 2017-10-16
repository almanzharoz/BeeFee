using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Microsoft.AspNetCore.Http;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateEventModel : CreateOrUpdateEventModel
    {
		[Required]
		public IFormFile File { get; set; }

        public CreateEventModel() { } // For binder

        public CreateEventModel(CompanyProjection company, IReadOnlyCollection<BaseCategoryProjection> categories)
        {
            CompanyId = company.Id;
            Email = company.Email;
            Init(company, categories);
        }
    }
}