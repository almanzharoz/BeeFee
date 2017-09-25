using BeeFee.JobsApp.Projections;
using BeeFee.JobsApp.Services;
using BeeFee.Model;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;

namespace BeeFee.JobsApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeJobsApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<MailService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeJobsApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services 
				.AddProjection<SendMailJob, Job<SendMail>>();

		}
	}
}
