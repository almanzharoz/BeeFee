using System;
using BeeFee.Model.Jobs;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections.Jobs
{
	public abstract class BaseJobProjection<T> : BaseEntityWithVersion, IJob<T>, IProjection<Job<T>> where T : struct
	{
		public DateTime Added { get; }
		public DateTime Start { get; }
		public DateTime Begin { get; private set; }
		public T Data { get; }
		public EJobState State { get; protected set; }

		protected BaseJobProjection(string id, int version, DateTime added, DateTime start, T data, EJobState state) : base(id, version)
		{
			Added = added;
			Start = start;
			Data = data;
			State = state;
		}

		public void Starting()
		{
			Begin = DateTime.UtcNow;
			State = EJobState.Doing;
		}

		public abstract void Execute(Action<T> action);
	}
}