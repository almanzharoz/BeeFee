using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.AdminApp.Projections.Category;
using BeeFee.AdminApp.Projections.Event;
using BeeFee.AdminApp.Projections.User;
using BeeFee.AdminApp.Services;
using BeeFee.Model;
using BeeFee.Model.Models;

namespace BeeFee.AdminApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeAdminApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventService>()
				.AddService<UserService>()
				.AddService<CategoryService>();
		}

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeAdminApp(this IElasticProjections<BeefeeElasticConnection> services)
		{
			return services
				.AddProjection<EventProjection, Event>()
				
				.AddProjection<NewUserProjection, User>()
				.AddProjection<UserUpdateProjection, User>()
				.AddProjection<UserProjection, User>()
				
				.AddProjection<CategoryProjection, Category>()
				.AddProjection<NewCategory, Category>();
		}
	}
}