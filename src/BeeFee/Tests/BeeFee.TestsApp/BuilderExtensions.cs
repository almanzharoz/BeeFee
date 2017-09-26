using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Models;
using BeeFee.Model.Projections.Jobs;
using BeeFee.TestsApp.Projections;
using BeeFee.TestsApp.Services;

namespace BeeFee.TestsApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeTestsApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<TestsJobsService>()
				.AddService<TestsUserService>()
				.AddService<TestsEventService>()
				.AddService<TestsCompanyService>()
                .AddService<TestsCaterogyService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeTestsApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<NewJob<SendMail>, Job<SendMail>>()
				.AddProjection<NewJob<CreateTicket>, Job<CreateTicket>>()

				.AddProjection<NewUser, User>()
				.AddProjection<NewEvent, Event>()
				.AddProjection<NewEventTransaction, EventTransaction>()
				.AddProjection<NewCompany, Company>()
				.AddProjection<NewCategory, Category>()
				.AddProjection<FullEvent, Event>()
				.AddProjection<EventJoinProjection, Event>()
				.AddProjection<FullEventTransaction, EventTransaction>();
		}
	}
}
