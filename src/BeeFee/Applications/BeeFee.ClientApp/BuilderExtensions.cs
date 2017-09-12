using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.ClientApp.Services;
using BeeFee.Model;
using BeeFee.Model.Models;

namespace BeeFee.ClientApp
{
    public static class BuilderExtensions
    {
        public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeClientApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
            return serviceRegistration
				.AddService<EventService>();
        }

        public static IElasticProjections<BeefeeElasticConnection> UseBeefeeClientApp(this IElasticProjections<BeefeeElasticConnection> services)
        {
            return services
                .AddProjection<EventProjection, Event>()
                .AddProjection<EventGridItem, Event>()
                .AddProjection<EventAddressProjection, Event>();

        }
    }
}