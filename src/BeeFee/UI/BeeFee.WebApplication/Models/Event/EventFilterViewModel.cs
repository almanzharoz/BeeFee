using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpFuncExt;

namespace BeeFee.WebApplication.Models.Event
{
    public class EventFilterViewModel
    {
		public EventFilterViewModel(LoadEventsRequest request, IReadOnlyCollection<string> cities, IReadOnlyCollection<CategoryProjection> categories, Pager<EventGridItem> events)
		{
			Cities = cities.Select(x => new SelectListItem {Text = x, Selected = request.City.IfNotNullOrDefault(x.Equals)}).ToArray();
			Categories = categories.Select(c => new SelectListItem {Value = c.Id, Text = c.Name, Selected = request.Categories?.Contains(c.Id) ?? false}).ToArray();
			StartDate = request.StartDate ?? DateTime.Now;
			EndDate = request.EndDate ?? DateTime.Now;
			Text = request.Text;
			Events = events;
		}

	    public SelectListItem[] Cities { get; }
        public SelectListItem[] Categories { get; }
        public DateTime StartDate{ get; }
        public DateTime EndDate { get; }
		public string Text { get; }

		public Pager<EventGridItem> Events { get; }

	}
}
