using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Projections.Event;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateOrUpdateEventDescriptionStepModel:CreateOrUpdateEventModel
    {
        public string Html { get; set; }

        public CreateOrUpdateEventDescriptionStepModel()
        {
            Step = 1;
        }

        public CreateOrUpdateEventDescriptionStepModel(EventProjection @event) : base(@event.Id, @event.Parent.Id, 1)
        {
            Html = @event.Page.Html;
            Version = @event.Version;
        }
    }
}
