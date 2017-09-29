using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Jobs.Data;
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
		protected TestsEventService _eventService;
		protected TestsCaterogyService _categoryService;
		protected TestsCompanyService _companyService;
		protected TestsJobsService _jobsService;

		protected BaseTestClass(
			Func<ServiceRegistration<BeefeeElasticConnection>, ServiceRegistration<BeefeeElasticConnection>> servicesRegistration,
			Func<IElasticProjections<BeefeeElasticConnection>, IElasticProjections<BeefeeElasticConnection>> useAppFunc,
			EUserRole[] roles)
		{
			_servicesRegistration = servicesRegistration;
			_useAppFunc = useAppFunc;
			_roles = roles;
		}

		protected virtual IServiceCollection AddServices(IServiceCollection serviceCollection)
			=> serviceCollection;

		public virtual void Setup()
		{
			var serviceProvider = AddServices(new ServiceCollection()
				.AddBeefeeModel(new Uri("http://localhost:9200/"), s => _servicesRegistration(s
					.AddBeefeeTestsApp()))
				.AddSingleton(x => new UserName(x.GetService<TestsUserService>().AddUser("user@mail.ru", "123", "user", _roles)))
				.AddLogging())
				.BuildServiceProvider();

			serviceProvider
				.UseBeefeeModel(s => _useAppFunc(s.UseBeefeeTestsApp()), true);

			Service = serviceProvider.GetService<TService>();

			_eventService = serviceProvider.GetService<TestsEventService>();
			_categoryService = serviceProvider.GetService<TestsCaterogyService>();
			_companyService = serviceProvider.GetService<TestsCompanyService>();
			_jobsService = serviceProvider.GetService<TestsJobsService>();

		}

		protected string AddEvent(string companyId, string categoryId, string name, EventDateTime date, EEventState state = EEventState.Created, Address address=default(Address), decimal price=0, int count = 10)
			=> _eventService.AddEvent(companyId, name, date, address, categoryId, state, price, count);

		protected string AddCategory(string name)
			=> _categoryService.Add(name);

		protected string AddCompany(string name)
			=> _companyService.Add(name);

		protected FullEvent GetEventById(string eventId, string companyId)
			=> _eventService.GetEventById(eventId, companyId);

		protected FullEventTransaction GetEventTransactionById(string eventId, string companyId)
			=> _eventService.GetEventTransactionById(eventId, companyId);

		protected bool AddSendMailJob(SendMail data, DateTime start)
			=> _jobsService.AddSendMailJob(data, start);

		protected bool AddCreateTicketJob(CreateTicket data, DateTime start)
			=> _jobsService.AddCreateTicketJob(data, start);

	}
}