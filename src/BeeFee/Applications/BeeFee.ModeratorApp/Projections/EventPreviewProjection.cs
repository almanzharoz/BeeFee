﻿using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.ModeratorApp.Projections
{
	public class EventPreviewProjection : BaseEntityWithParentAndVersion<BaseCompanyProjection>, IEventPageProjection, IGetProjection
	{
		public string Url { get; }

		public string Name { get; }

		public BaseCategoryProjection Category { get; }

		public EEventState State { get; }

		public EventPage Page { get; }

		public EventPreviewProjection(string id, BaseCompanyProjection company, string url, string name, BaseCategoryProjection category, EEventState state, EventPage page, int version)
			: base(id, company, version)
		{
			Url = url;
			Name = name;
			Category = category;
			State = state;
			Page = page;
		}
	}
}