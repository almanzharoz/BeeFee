using System;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Projections.Jobs;
using Core.ElasticSearch.Domain;

namespace BeeFee.JobsApp.Projections
{
	public class SendMailJob : BaseJobProjection<SendMail>, ISearchProjection, IUpdateProjection
	{
		public DateTime Done { get; private set; }
		public string Exception { get; private set; }

		public SendMailJob(string id, int version, DateTime added, DateTime start, SendMail data, EJobState state) 
			: base(id, version, added, start, data, state)
		{
		}

		public override void Execute(Action<SendMail> action)
		{
			try
			{
				action(this.Data);
				State = EJobState.Done;
			}
			catch (Exception e)
			{
				Exception = e.Message + "\r\n" + e.StackTrace;
				State = EJobState.Error;
			}
			finally
			{
				Done = DateTime.UtcNow;
			}
		}
	}
}