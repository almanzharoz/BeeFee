using System;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.ModeratorApp.Projections.Embed;
using Core.ElasticSearch.Domain;

namespace BeeFee.ModeratorApp.Projections
{
	public class EventModeratorProjection : BaseEntityWithParentAndVersion<BaseCompanyProjection>, IProjection<Event>, IGetProjection, IUpdateProjection
	{
		public string Name { get; private set; }
		public EEventType Type { get; private set; }
		public EventPageModerate Page { get; private set; }
		public BaseCategoryProjection Category { get; private set; }


		public EventModeratorProjection(string id, BaseCompanyProjection parent, int version) : base(id, parent, version)
		{
		}

		internal EventModeratorProjection Change(string title, string label, string caption, Address address, BaseCategoryProjection category, string html)
		{
			Category = category;
			this.Page.Change(title, label, caption, address, category.Name, html);
			return this;
		}

		internal EventModeratorProjection ChangeType(bool moderated)
		{
			if (Type != EEventType.Moderating)
				throw new Exception("Event is not in moderating state");
			Type = moderated ? EEventType.Open : EEventType.NotModerated;
			return this;
		}
	}
}