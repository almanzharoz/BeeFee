using System;
using System.Threading.Tasks;
using BeeFee.Model;
using BeeFee.Model.Jobs;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.JobsApp.Services
{
	public abstract class BaseJobsService<TJob, TData> : BaseBeefeeService 
		where TJob : BaseEntityWithVersion, IJob<TData>, IProjection, ISearchProjection , IUpdateProjection
		where TData : struct
	{
		protected BaseJobsService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		protected TJob GetNextJob()
			=> UpdateWithFilterAndVersion<TJob>(
				q => q.Term(p => p.State, EJobState.New) && q.DateRange(d => d.Field(p => p.Start).LessThanOrEquals(DateMath.Now)),
				s => s.Ascending(p => p.Start), j => j.Fluent(j.Starting), true);

		protected Task<bool> JobExecute(TJob job, Func<TData, Task> action)
			=> Task.FromResult(job.IfNotNull(
				x => UpdateWithVersion(x, j => j.Fluent(f =>
				{
					try
					{
						f.Execute(d => action.HasNotNullArg(nameof(action))(d));
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}), false), () => false));

	}
}