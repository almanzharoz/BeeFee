using System;
using System.Linq;
using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using BeeFee.TestsApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.TestsApp.Services
{
	public class TestsEventService: BaseBeefeeService
	{
		public TestsEventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public string AddEvent(string companyId, string name, EventDateTime dateTime, Address address, string categoryId, EEventType type, decimal price, int count=10)
		{
			var category = Get<BaseCategoryProjection>(categoryId.HasNotNullArg(nameof(categoryId))).HasNotNullArg("category");

			return new NewEvent(Get<BaseCompanyProjection>(companyId), Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"), name)
				{
					DateTime = dateTime,
					Address = address,
					Type = type,
					Category = category,
					Page = new EventPage(name, "label", category.Name, "", dateTime.ToString(), address, "<p>Html text</p>"),
					Prices = new TicketPrice[1] {new TicketPrice(Guid.Empty, "Ticket", "default", price, count, count)}
				}
				.Fluent(x => Insert<NewEvent, BaseCompanyProjection>(x, true))
				.Fluent(x => Insert(new NewEventTransaction(x), true)).Id;
		}

		public FullEvent GetEventById(string eventId, string companyId)
			=> GetWithVersion<FullEvent, BaseCompanyProjection>(eventId, companyId);

		public FullEventTransaction GetEventTransactionById(string eventId, string companyId)
			=> Filter<FullEventTransaction>(q =>
				q.Term(p => p.Event, eventId.HasNotNullArg(nameof(eventId))) &&
				q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId)))).FirstOrDefault();
	}
}