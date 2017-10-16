using BeeFee.JobsApp.Projections;
using BeeFee.JobsApp.Services;
using BeeFee.Model;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Projections.Jobs;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;

namespace BeeFee.JobsApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeJobsApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<MailService>()
				.AddService<TicketService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeJobsApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services 
                .AddProjection<NewJob<SendMail>, Job<SendMail>>()
				.AddProjection<SendMailJob, Job<SendMail>>()
				.AddProjection<CreateTicketJob, Job<CreateTicket>>();

		}
	}
}
