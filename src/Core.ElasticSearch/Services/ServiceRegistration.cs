using Microsoft.Extensions.DependencyInjection;

namespace Core.ElasticSearch
{
	public class ServiceRegistration<TConnection> where TConnection : BaseElasticConnection
	{
		private readonly IServiceCollection _services;
		internal ServiceRegistration(IServiceCollection services)
		{
			_services = services;
		}

		public ServiceRegistration<TConnection> AddService<TService>() where TService : BaseService<TConnection>
		{
			_services.AddScoped<TService>();
			return this;
		}
	}
}