using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.OrganizerApp.Projections.Company;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public abstract class CreateOrUpdateEventModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Company is required")]
        public string CompanyId { get; set; }

        public int Step { get; set; }

        public int Version { get; set; }

        public EEventState State { get; private set; }

        public bool IsNew => string.IsNullOrEmpty(Id);

        public CreateOrUpdateEventModel()
        {
            State = EEventState.Created;
        }

        public CreateOrUpdateEventModel(string id, string companyId, int step)
        {
            this.Id = id;
            this.CompanyId = companyId;
            this.Step = step;
            State = EEventState.Created;
        }
    }
}
