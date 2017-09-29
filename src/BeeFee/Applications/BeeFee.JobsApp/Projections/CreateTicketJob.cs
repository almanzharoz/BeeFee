using System;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Projections.Jobs;
using Core.ElasticSearch.Domain;

namespace BeeFee.JobsApp.Projections
{
	public class CreateTicketJob : BaseJobProjection<CreateTicket>, ISearchProjection, IUpdateProjection
	{

		public CreateTicketJob(string id, int version, DateTime added, DateTime start, CreateTicket data, EJobState state) 
			: base(id, version, added, start, data, state)
		{
		}

	}
}