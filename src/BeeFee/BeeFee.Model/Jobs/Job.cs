using System;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Jobs
{
	public abstract class Job<T> : IModel where T : struct
	{
		public string Id { get; set; }
		public DateTime Added { get; set; }
		public DateTime Start { get; set; }
		public DateTime Begin { get; set; }
		public DateTime Done { get; set; }
		public T Data { get; set; }

		public EJobState State { get; set; }
		public string Exception { get; set; }
	}
}