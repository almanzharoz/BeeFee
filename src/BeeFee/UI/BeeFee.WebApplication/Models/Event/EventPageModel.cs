using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ClientApp.Projections.Event;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BeeFee.WebApplication.Models.Event
{
    public class EventPageModel
    {
        public EventProjection Event { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public int? RegisterResult { get; set; }
    }
}
