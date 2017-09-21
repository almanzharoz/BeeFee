using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.TestsApp.Projections;
using BeeFee.TestsApp.Services;

namespace BeeFee.TestsApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeTestsApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<TestsUserService>()
				.AddService<TestsEventService>()
				.AddService<TestsCompanyService>()
                .AddService<TestsCaterogyService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeTestsApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<NewUser, User>()
				.AddProjection<NewEvent, Event>()
				.AddProjection<NewCompany, Company>()
				.AddProjection<NewCategory, Category>()
				.AddProjection<FullEvent, Event>();
		}
	}
}
