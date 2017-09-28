using System;
using System.ComponentModel.DataAnnotations;
using BeeFee.Model.Embed;

namespace BeeFee.WebApplication.Models.Event
{
    public class LoadEventsRequest
    {
        public string Text { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate{ get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string City{ get; set; }
        public int PageIndex { get; set; }
        public int? PageSize { get; set; }
        public decimal? MaxPrice { get; set; }
        public string[] Categories { get; set; }
    }
}
