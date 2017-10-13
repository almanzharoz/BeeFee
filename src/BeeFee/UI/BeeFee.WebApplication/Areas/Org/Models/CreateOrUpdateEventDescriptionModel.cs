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
    public class CreateOrUpdateEventDescriptionModel
    {
        public CreateOrUpdateEventDescriptionModel()
        {
        }

        public CreateOrUpdateEventDescriptionModel(CreateOrUpdateEventModel model)
        {
            this.Html = model.Html;
        }

        public string Html { get; set; }
    }
}
