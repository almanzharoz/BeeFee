using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.OrganizerApp.Projections.Event;
using BeeFee.OrganizerApp.Services;

namespace BeeFee.OrganizerApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeOrganizerApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeOrganizerApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<EventProjection, Event>()
				.AddProjection<NewEvent, Event>();
		}
	}
}