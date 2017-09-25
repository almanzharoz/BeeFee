using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model.Embed;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.Model
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddBeefeeModel(this IServiceCollection services, Uri connectionString,
		    Action<ServiceRegistration<BeefeeElasticConnection>> servicesRegistration)
		    => services.AddElastic(new BeefeeElasticConnection(connectionString),
			    s => servicesRegistration(s
				    .AddService<CategoryService>()));

	    public static IServiceProvider UseBeefeeModel(this IServiceProvider services, Func<IElasticProjections<BeefeeElasticConnection>, IElasticProjections<BeefeeElasticConnection>> projectionsRegistration, bool forTest = false)
		    => services.UseElastic<BeefeeElasticConnection>(AddMapping, projectionsRegistration, forTest);

	    private static void AddMapping(IElasticMapping<BeefeeElasticConnection> m)
		    => m
			    // маппинг
			    .AddMapping<User>(x => x.UserIndexName)
			    .AddMapping<Event>(x => x.EventIndexName)
			    .AddMapping<EventTransaction>(x => x.EventIndexName)
			    .AddMapping<Category>(x => x.EventIndexName)
			    .AddMapping<Company>(x => x.EventIndexName)

				.AddMapping<Job<SendMail>>(x => x.JobsIndexName)

				// внутренние документы
				.AddStruct<TicketPrice>()
			    .AddStruct<Address>()
			    .AddStruct<EventDateTime>()
			    .AddStruct<EventPage>()
			    .AddStruct<CompanyUser>()
				// проекции
				.AddProjection<BaseUserProjection, User>()
			    .AddProjection<BaseCategoryProjection, Category>()
			    .AddProjection<BaseCompanyProjection, Company>();
    }
}
