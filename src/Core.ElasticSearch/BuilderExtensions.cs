using System;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddElastic<TConnection>(this IServiceCollection services, TConnection settings, Action<ServiceRegistration<TConnection>> servicesRegistration)
			where TConnection : BaseElasticConnection
		{
			services
				.AddSingleton<TConnection>(settings)
				.AddSingleton<ElasticMapping<TConnection>>()
				.AddScoped<ElasticScopeFactory<TConnection>>();
			if (servicesRegistration != null)
				servicesRegistration(new ServiceRegistration<TConnection>(services));
			return services;
		}

		public static IServiceProvider UseElastic<TConnection, TService>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, services.GetService<TService>());
			return services;
		}

		public static IServiceProvider UseElastic<TConnection>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, Func<IElasticProjections<TConnection>, IElasticProjections<TConnection>> projectionsRegistration, bool forTest)
			where TConnection : BaseElasticConnection
		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			if (projectionsRegistration != null)
				projectionsRegistration(mapping);
			if (forTest)
				mapping.Drop();
			mapping.Build(null);
			return services;
		}

		public static IServiceProvider UseElastic<TConnection, TService>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, bool forTest, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>
		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			if (forTest)
				mapping.Drop();
			mapping.Build(initFunc, services.GetService<TService>());
			return services;
		}
	}
}