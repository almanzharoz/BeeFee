﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public EventFilterViewModel(LoadEventsRequest request, IReadOnlyCollection<string> cities, IReadOnlyCollection<BaseCategoryProjection> categories, (bool allLoaded, string html) loadEventsResult)
        {
            Cities = cities.Select(x => new SelectListItem { Text = x, Selected = request.City.NotNullOrDefault(x.Equals) }).ToArray();
            Categories = categories.Select(c => new SelectListItem { Value = c.Id, Text = c.Name, Selected = request.Categories?.Contains(c.Id) ?? false }).ToArray();
            StartDate = request.StartDate ?? DateTime.Now;
            EndDate = request.EndDate ?? DateTime.Now;
            Text = request.Text;
            City = request.City;
            MaxPrice = request.MaxPrice;
            Types = request.Types;
            EventHtml = loadEventsResult.html;
            AllLoaded = loadEventsResult.allLoaded;
            //Events = events;
        }

        public SelectListItem[] Cities { get; }
        public SelectListItem[] Categories { get; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; }
        public string Text { get; }
        public string City { get; }
        public decimal? MaxPrice { get; }
        public EEventType[] Types { get; }

        public string EventHtml { get; }
        public bool AllLoaded { get; }

        //public Pager<EventGridItem> Events { get; }

    }
}
