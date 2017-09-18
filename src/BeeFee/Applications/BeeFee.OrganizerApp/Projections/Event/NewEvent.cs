﻿using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections.Event
{
	internal class NewEvent : BaseNewEntityWithParent<CompanyJoinProjection>, IProjection<Model.Models.Event>, IWithName, IWithOwner
	{
		public string Name { get; }
		public string Url { get; }

		public EventDateTime DateTime { get; }

		public EEventType Type { get; }

		public Address Address { get; }

		public EventPage Page { get; }

		public BaseCategoryProjection Category { get; }

		public BaseUserProjection Owner { get; }
		public TicketPrice[] Prices { get; }

		private readonly ThrowCollection _throws = new ThrowCollection();

		public NewEvent(CompanyJoinProjection company, BaseUserProjection owner, BaseCategoryProjection category, string name, string label, string url, string cover, EEventType type,
			EventDateTime dateTime, Address address, TicketPrice[] prices, string html) : base(company)
		{
			Owner = owner.HasNotNullEntity(_throws, nameof(owner));
			Category = category.HasNotNullEntity(_throws, nameof(category));
			Name = name.HasNotNullArg(_throws, nameof(name));
			Url = url.IfNull(name, CommonHelper.UriTransliterate);
			DateTime = dateTime;
			Type = type;
			Address = address;
			Prices = prices;
			Page = new EventPage(name, label, category.Name, cover, dateTime.ToString(), address, html);
			_throws.Throw();
		}
	}
}