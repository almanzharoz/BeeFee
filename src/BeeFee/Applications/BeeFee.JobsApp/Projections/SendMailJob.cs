using System;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Projections.Jobs;
using Core.ElasticSearch.Domain;

namespace BeeFee.JobsApp.Projections
{
	public class SendMailJob : BaseJobProjection<SendMail>, ISearchProjection, IUpdateProjection
	{

		public SendMailJob(string id, int version, DateTime added, DateTime start, SendMail data, EJobState state) 
			: base(id, version, added, start, data, state)
		{
		}

	}
}