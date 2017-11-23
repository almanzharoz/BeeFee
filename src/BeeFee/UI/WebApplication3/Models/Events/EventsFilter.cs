using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpFuncExt;

namespace WebApplication3.Models.Events
{
	public class EventsFilter : PagerFilter<EventGridItem>
	{
		public SelectListItem[] Cities { get; private set; }
		public SelectListItem[] Categories { get; private set; }

		public string Text { get; set; }
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime? StartDate { get; set; }
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime? EndDate { get; set; }
		public string City { get; set; }
		public string[] Category { get; set; }

		public EventsFilter() : base(9)
		{
		}

		public EventsFilter Load(Pager<EventGridItem> items)
		{
			LoadItems(items);
			return this;
		}

		public EventsFilter Init(IReadOnlyCollection<string> cities, IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			Cities = cities.Where(x => x != null).Select(x => new SelectListItem { Text = x, Selected = City.NotNullOrDefault(x.Equals) }).ToArray();
			Categories = categories.Select(c => new SelectListItem { Value = c.Id, Text = c.Name, Selected = Category != null && Category.Contains(c.Id) }).ToArray();
			return this;
		}
	}
}