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
    public class CreateOrUpdateEventPreviewStepModel : CreateOrUpdateEventModel
    {
        public IEventPageProjection Preview { get; private set; }

        public CreateOrUpdateEventPreviewStepModel()
        {
            Step = 3;
        }

        public CreateOrUpdateEventPreviewStepModel(IEventPageProjection @event, int version) : base(@event.Id, @event.Parent.Id, 3)
        {
            Version = version;
            Preview = @event.HasNotNullEntity("Event");
        }
    }
}
