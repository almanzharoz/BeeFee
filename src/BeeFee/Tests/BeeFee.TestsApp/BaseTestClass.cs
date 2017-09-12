using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.TestsApp.Projections;
using BeeFee.TestsApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.TestsApp
{
	public abstract class BaseTestClass<TService> where TService : BaseBeefeeService
	{
		protected TService Service { get; private set; }

		private readonly Func<ServiceRegistration<BeefeeElasticConnection>, ServiceRegistration<BeefeeElasticConnection>> _servicesRegistration;
		private readonly Func<IElasticProjections<BeefeeElasticConnection>, IElasticProjections<BeefeeElasticConnection>> _useAppFunc;
		private readonly EUserRole[] _roles;
		private TestsEventService _eventService;
		private TestsCaterogyService _categoryService;

		protected BaseTestClass(
			Func<ServiceRegistration<BeefeeElasticConnection>, ServiceRegistration<BeefeeElasticConnection>> servicesRegistration,
			Func<IElasticProjections<BeefeeElasticConnection>, IElasticProjections<BeefeeElasticConnection>> useAppFunc,
			EUserRole[] roles)
		{
			_servicesRegistration = servicesRegistration;
			_useAppFunc = useAppFunc;
			_roles = roles;
		}

		public virtual void Setup()
		{
			var serviceProvider = new ServiceCollection()
				.AddBeefeeModel(new Uri("http://localhost:9200/"), s => _servicesRegistration(s
					.AddBeefeeTestsApp()))
				.AddSingleton(x => new UserName(x.GetService<TestsUserService>().AddUser("user@mail.ru", "123", "user", _roles)))
				.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseBeefeeModel(s => _useAppFunc(s.UseBeefeeTestsApp()), true);

			Service = serviceProvider.GetService<TService>();

			_eventService = serviceProvider.GetService<TestsEventService>();
			_categoryService = serviceProvider.GetService<TestsCaterogyService>();

		}

		protected string AddEvent(string categoryId, string name, EventDateTime date, Address address=default(Address), EEventType type= EEventType.None, decimal price=0)
			=> _eventService.AddEvent(name, date, address, type, categoryId, price);

		protected string AddCategory(string name)
			=> _categoryService.Add(name);
	}
}