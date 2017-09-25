using System;
using BeeFee.Model.Jobs;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.Model.Projections.Jobs
{
	public class NewJob<T> : BaseNewEntity, IProjection<Job<T>> where T : struct
	{
		public DateTime Added { get; }
		public DateTime Start { get; }
		public EJobState State { get; }
		public T Data { get; }

		public NewJob(T data, DateTime start)
		{
			Data = data;
			Added = DateTime.UtcNow;
			State = EJobState.New;
			Start = start.ThrowIf(x => (DateTime.UtcNow - x).TotalMilliseconds > 1000, x => new Exception("Start date < now"));
		}
	}
}