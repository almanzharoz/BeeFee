using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.LoginApp.Projections.User;
using BeeFee.LoginApp.Services;
using BeeFee.Model;
using BeeFee.Model.Models;

namespace BeeFee.LoginApp
{
    public static class BuilderExtensions
	{
		public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeLoginApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
			=> serviceRegistration
				.AddService<AuthorizationService>();

		public static IElasticProjections<BeefeeElasticConnection> UseBeefeeLoginApp(this IElasticProjections<BeefeeElasticConnection> services)
			=> services
				.AddProjection<UserProjection, User>()
				.AddProjection<UserUpdateProjection, User>()
				.AddProjection<RegisterUserProjection, User>();
	}
}
