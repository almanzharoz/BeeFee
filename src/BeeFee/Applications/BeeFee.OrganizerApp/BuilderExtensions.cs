using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Projections.Event;
using BeeFee.OrganizerApp.Services;

namespace BeeFee.OrganizerApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeOrganizerApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventService>()
				.AddService<CompanyService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeOrganizerApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<CompanyJoinProjection, Company>()
				.AddProjection<CompanyProjection, Company>()
				.AddProjection<NewCompany, Company>()
				.AddProjection<EventProjection, Event>()
				.AddProjection<EventPreviewProjection, Event>()
				.AddProjection<EventTransactionProjection, EventTransaction>()
				.AddProjection<EventJoinProjection, Event>()
				.AddProjection<NewEvent, Event>()
				.AddProjection<NewEventTransaction, EventTransaction>();
		}
	}
}