using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateEventModel : CreateOrUpdateEventModel
    {
        public CreateEventModel() { } // For binder

        public CreateEventModel(CompanyProjection company, IReadOnlyCollection<BaseCategoryProjection> categories)
        {
            CompanyId = company.Id;
            Email = company.Email;
            Init(categories);
        }

        public new CreateEventModel Init(IReadOnlyCollection<BaseCategoryProjection> categories)
        {
            base.Init(categories);
            return this;
        }
    }
}