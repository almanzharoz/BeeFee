using System;
using System.Threading.Tasks;
using BeeFee.Model.Jobs;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections.Jobs
{
	public abstract class BaseJobProjection<T> : BaseEntityWithVersion, IJob<T>, IProjection<Job<T>> where T : struct
	{
		public DateTime Added { get; }
		public DateTime Start { get; }
		public DateTime Begin { get; private set; }
		public DateTime Done { get; private set; }
		public T Data { get; }
		public EJobState State { get; protected set; }
		public string Exception { get; private set; }

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

		public virtual async Task Execute(Func<T, Task> action)
		{
			try
			{
				await action(this.Data);
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