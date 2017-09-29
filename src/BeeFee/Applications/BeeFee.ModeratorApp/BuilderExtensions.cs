using System;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.ModeratorApp.Projections;
using BeeFee.ModeratorApp.Services;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;

namespace BeeFee.ModeratorApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeModeratorApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventModeratorService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeModeratorApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<EventModeratorGridItem, Event>()
				.AddProjection<EventModeratorProjection, Event>();
		}
	}
}
