using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Event;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication3.Areas.Org.Models.Event
{
	public interface IEventEditModel { }

	public class EventEditModel : IEventEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Label is required")]
		public string Label { get; set; }

		// TODO: Добавить клиентскую Remote-проверку
		[RegularExpression(@"[a-zA-Z-_\d]{1,}", ErrorMessage = "Доступны только латинские буквы, цифры и символы \"_\", \"-\"")]
		public string Url { get; set; }

		[Required(ErrorMessage = "Category is required")]
		public string CategoryId { get; set; }

		[Required(ErrorMessage = "Start date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime StartDateTime { get; set; }

		[Required(ErrorMessage = "Finish date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FinishDateTime { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }

		[Required(ErrorMessage = "Email is required"), EmailAddress]
		public string Email { get; set; }

		public IFormFile File { get; set; }
		public string Cover { get; set; }

		public string CompanyUrl { get; set; }

		public IList<SelectListItem> Categories { get; private set; }

		public EventEditModel() { }

		public EventEditModel(EventProjection @event, IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			CompanyUrl = @event.Parent.Url;
			Name = @event.Name;
			Label = @event.Page.Label;
			Url = @event.Url;
			CategoryId = @event.Category.Id;
			StartDateTime = @event.DateTime.Start;
			FinishDateTime = @event.DateTime.Finish;
			City = @event.Address.City;
			Address = @event.Address.AddressString;
			Cover = @event.Page.Cover;
			Email = @event.Email;
			Init(categories);
		}

		public EventEditModel Init(IReadOnlyCollection<BaseCategoryProjection> categories)
		{
			Categories = categories.Select(x => new SelectListItem { Text = x.Name, Value = x.Id, Selected = x.Id == CategoryId }).ToList();
			return this;
		}

	}
}