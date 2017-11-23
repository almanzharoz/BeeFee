using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.ClientApp.Projections.User;
using BeeFee.ClientApp.Services;
using BeeFee.Model;
using BeeFee.Model.Jobs;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Models;
using BeeFee.Model.Projections.Jobs;

namespace BeeFee.ClientApp
{
    public static class BuilderExtensions
    {
        public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeClientApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
            return serviceRegistration
				.AddService<EventService>()
				.AddService<UserService>();
        }

        public static IElasticProjections<BeefeeElasticConnection> UseBeefeeClientApp(this IElasticProjections<BeefeeElasticConnection> services)
        {
            return services
                .AddProjection<NewJob<SendMail>, Job<SendMail>>()
                .AddProjection<NewJob<CreateTicket>, Job<CreateTicket>>()
                .AddProjection<EventProjection, Event>()
				.AddProjection<UserUpdateProjection, User>()
                .AddProjection<EventJoinProjection, Event>()
                .AddProjection<EventGridItem, Event>()
                .AddProjection<EventAddressProjection, Event>()
                .AddProjection<EventTransactionPricesProjection, EventTransaction>()
                .AddProjection<EventTransactionProjection, EventTransaction>()
				.AddProjection<RegisterToEventProjection, EventTransaction>();

        }
    }
}