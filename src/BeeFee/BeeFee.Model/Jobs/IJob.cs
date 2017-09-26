using System;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Jobs
{
	/// <summary>
	/// T должно быть одинаково у всех проекций одной задачи
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IJob<T> : IEntity where T : struct
	{
		DateTime Added { get; }
		DateTime Start { get; }
		T Data { get; }

		EJobState State { get; }

		void Starting();
		void Execute(Action<T> action);
	}
}