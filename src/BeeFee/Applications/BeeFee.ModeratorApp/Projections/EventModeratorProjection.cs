using System;
using System.Linq;
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
		public EEventState State { get; private set; }
		public EventPageModerate Page { get; private set; }
		public BaseCategoryProjection Category { get; private set; }
		public string[] Comments { get; private set; }


		public EventModeratorProjection(string id, BaseCompanyProjection parent, int version) : base(id, parent, version)
		{
		}

		internal EventModeratorProjection Change(string title, string label, string caption, Address address, BaseCategoryProjection category, string html)
		{
			Category = category;
			this.Page.Change(title, label, caption, address, category.Name, html);
			return this;
		}

		internal EventModeratorProjection Moderate(string comment, bool moderated)
		{
			if (State != EEventState.Moderating)
				throw new Exception("Event is not in moderating state");
			State = moderated ? EEventState.Open : EEventState.NotModerated;
			if (!String.IsNullOrWhiteSpace(comment))
				Comments = (Comments ?? new string[0]).Union(new [] {comment}).ToArray();
			return this;
		}
	}
}