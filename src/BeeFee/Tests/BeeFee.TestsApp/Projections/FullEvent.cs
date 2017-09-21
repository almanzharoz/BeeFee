using System;
using System.Collections.Generic;
using System.Text;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.TestsApp.Projections
{
    public class FullEvent : BaseEntityWithParentAndVersion<BaseCompanyProjection>, IProjection<Event>, IGetProjection
    {

		public string Url { get; private set; }

		public string Name { get; private set; }

		public BaseCategoryProjection Category { get; private set; }

		public EEventType Type { get; private set; }

		public TicketPrice[] Prices { get; private set; }

		public int TicketsLeft { get; private set; }

		public EventPage Page { get; private set; }

		public Transaction[] Transactions { get; private set; }

		public FullEvent(string id, BaseCompanyProjection parent, int version) : base(id, parent, version)
		{
		}
	}
}
