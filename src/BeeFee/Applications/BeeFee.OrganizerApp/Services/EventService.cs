using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.OrganizerApp.Projections.Event;
using Core.ElasticSearch.Domain;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Services
{
    public class EventService : BaseBeefeeService
    {
        public EventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
            ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
        {
        }

		public T GetCompany<T>(string id)
			where T : BaseEntityWithVersion, ISearchProjection, IGetProjection, IProjection<Company>
			=> id.IfNotNull(
				c => GetWithVersionByIdAndQuery<Company, T>(c, f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id)),
				() => Filter<Company, T>(f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id)).FirstOrDefault());

		public Task<T> GetCompanyAsync<T>(string id)
			where T : BaseEntityWithVersion, ISearchProjection, IGetProjection, IProjection<Company>
			=> id.IfNotNull(
				c => GetWithVersionByIdAndQueryAsync<Company, T>(c, f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id)),
				() => FilterFirstAsync<Company, T>(f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id)));

		public EventProjection GetEvent(string id, string company)
            => GetWithVersionByIdAndQuery<Event, EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

		public Task<EventProjection> GetEventAsync(string id, string company)
			=> GetWithVersionByIdAndQueryAsync<Event, EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

		public EventPreviewProjection GetPreviewEvent(string id, string company)
            => GetByIdAndQuery<Event, EventPreviewProjection, BaseCompanyProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

		public Task<EventPreviewProjection> GetPreviewEventAsync(string id, string company)
			=> GetByIdAndQueryAsync<Event, EventPreviewProjection, BaseCompanyProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

		public Task<TicketPrice> GetEventTicketAsync(string id, string companyId, string ticketId)
			=> FilterNestedFirstAsync<EventTransaction, TicketPrice, EventTransactionProjection, TicketPrice>(
				q => q.Term(p => p.Event, id.HasNotNullArg(nameof(id))) &&
					 q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId))),
				p => p.Prices,
				q => q.Term(x => x.Prices.First().Id, ticketId));

		///// <exception cref="AddEntityException"></exception>
		//// TODO: добавить проверку 
		//public string AddEvent(string companyId, string categoryId, string name, string label, string url, string email,
  //          EventDateTime dateTime, Address address, TicketPrice[] prices, string html, string cover)
  //      {
  //          // TODO: Проверять и вставлять одним запросом
  //          url.IfNull(name, CommonHelper.UriTransliterate)
  //              .ThrowIf(ExistsByUrl<EventProjection>, x => new ExistsUrlException<Event>(x));

  //          var newEvent = new NewEvent(
  //              companyId.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)),
  //              GetById<BaseUserProjection>(User.Id).HasNotNullEntity("user"),
  //              GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category"), name, label, url,
  //              dateTime, address, email, cover);

  //          Insert<NewEvent, CompanyJoinProjection>(newEvent, true).ThrowIfNot<AddEntityException<Event>>();

  //          Insert(new NewEventTransaction(newEvent), false).ThrowIfNot<AddEntityException<EventTransaction>>(); // TODO: при такой ошибке должен быть откат

  //          return newEvent.Id;
//        }

		public async Task<BoolResult<string>> AddEventAsync(string companyId, string categoryId, string name, string label,
			string url, string email,
			EventDateTime dateTime, Address address, string cover)
		{
			// TODO: Проверять и вставлять одним запросом
			// Вставить одним запросов не получится, пологаю лучшим вариантом будет, после вставки проверить нет ли другого документа с таким url и откатить этот

			var newEvent = new NewEvent(
				companyId.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)),
				GetById<BaseUserProjection>(User.Id).HasNotNullEntity("user"),
				GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category"),
				name, label,
				url.IfNull(name, CommonHelper.UriTransliterate)
					.ThrowIf(ExistsByUrl<EventProjection>, x => new ExistsUrlException<Event>(x)),
				dateTime, address, email, cover);

			NewEventTransaction newEventTransaction = null;

			return new BoolResult<string>(await
					newEvent.Rollback(
						async e => await InsertAsync<NewEvent, CompanyJoinProjection>(e, true) &&
									!ExistsByUrl<EventProjection>(e.Id, e.Url) &&
									await InsertAsync(newEventTransaction = new NewEventTransaction(e), true),
						e => e.Id.NotNullOrDefault(id =>
							Remove<EventProjection, CompanyJoinProjection>(e.Id, e.Parent.Id, 1, false) &&
							Remove<EventTransactionProjection>(newEventTransaction.Id, false))),
				newEvent.Id);
		}

		private EventTransactionProjection GetEventTransactionById(string eventId, string companyId)
            => Filter<EventTransactionProjection>(q =>
                q.Term(p => p.Event, eventId.HasNotNullArg(nameof(eventId))) &&
                q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId)))).FirstOrDefault();

		public Task<EventTransactionPricesProjection> GetEventTransactionByIdAsync(string eventId, string companyId)
			=> FilterFirstAsync<EventTransactionPricesProjection>(q =>
				q.Term(p => p.Event, eventId.HasNotNullArg(nameof(eventId))) &&
				q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId))));

		public Task<bool> AddEventTicketPriceAsync(string id, string company, string name, string description, decimal price, int count, string template)
			=> UpdateWithFilterAsync<EventTransactionPricesProjection>(q => q.Term(p => p.Event, id) && q.Term(p => p.Company, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id),
				null, x => x.AddPrice(name, description, price, count, template), true);

		public Task<bool> UpdateEventTicketPriceAsync(string id, string company, string tickerPriceId, string name, string description, decimal price, int count, string template)
			=> UpdateWithFilterAsync<EventTransactionPricesProjection>(q => q.Term(p => p.Event, id) && q.Term(p => p.Company, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id),
				null, x => x.UpdatePrice(tickerPriceId, name, description, price, count, template), true);

		public Task<bool> RemoveTicketPriceAsync(string id, string company, string tickerPriceId)
			=> UpdateWithFilterAsync<EventTransactionPricesProjection>(q => q.Term(p => p.Event, id) && q.Term(p => p.Company, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id),
				null, x => x.RemovePrice(tickerPriceId), true);

		///<exception cref="RemoveEntityException"></exception>
		public bool RemoveEvent(string id, string company, int version)
            => Remove<EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
                q => q.Term(p => p.State, EEventState.Created), true)
            && Remove(GetEventTransactionById(id, company), false);

        public bool CloseEvent(string id, string companyId, int version)
            => UpdateById<EventProjection, CompanyJoinProjection>(id,
                    companyId.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version, u => u.Close(), true);

        /// <exception cref="UpdateEntityException"></exception>
        /// <exception cref="EntityAccessException{T}"></exception>
        /// <exception cref="ArgumentNullException">companyId, user, category</exception>
        /// <exception cref="EventStateException"></exception>
        /// <exception cref="Core.ElasticSearch.Exceptions.VersionException"></exception>
        public bool UpdateEvent(string id, string company, int version, string name, string label, string url, string cover, string email,
            EventDateTime dateTime, Address address,
            string categoryId)
            => UpdateById<EventProjection, CompanyJoinProjection>(id,
                    company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
                    x => x.Change(name, label, url, cover, email, dateTime, address,
                        GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category")), true);

		public Task<bool> UpdateEventAsync(string id, string company, int version, string name, string label, string url, string cover, string email,
			EventDateTime dateTime, Address address,
			string categoryId)
			=> UpdateByIdAsync<EventProjection, CompanyJoinProjection>(id,
				company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
				x => x.Change(name, label, url, cover, email, dateTime, address,
					GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category")), true);

		public bool UpdateEvent(string id, string company, int version, string html)
            => UpdateById<EventProjection, CompanyJoinProjection>(id,
                company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
                x => x.ChangeHtml(html), true);

		public Task<bool> UpdateEventDescriptionAsync(string id, string company, int version, string html)
			=> UpdateByIdAsync<EventProjection, CompanyJoinProjection>(id,
				company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
				x => x.ChangeHtml(html), true);

		public IReadOnlyCollection<EventProjection> GetMyEvents(string companyId)
            => Filter<Event, EventProjection>(q => UserQuery<EventProjection>(x => x.HasParent<Company>(p => p.Query(pq => pq.Ids(id => id.Values(companyId.HasNotNullArg("company")))))));

		public Task<Pager<EventProjection>> GetMyEventsAsync(string companyId, int page, int limit)
			=> FilterPagerAsync<Event, EventProjection>(
				q => UserQuery<EventProjection>(x =>
					x.HasParent<Company>(p => p.Query(pq => pq.Ids(id => id.Values(companyId.HasNotNullArg("company")))))),
				page, limit, s => s.Descending(x => x.DateTime.Start));

		/// <summary>
		/// Отправить мероприятие на модерацию
		/// </summary>
		/// <returns></returns>
		/// <exception cref="EntityAccessException{T}"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public bool ToModerate(string id, string company, int version)
            => UpdateById<EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version, x => x.ToModerate(), true);

		public IReadOnlyCollection<EventTicketTransaction> GetRegisteredUsers(string id, string companyId, int page, int take)
			=> FilterNested<EventTransaction, TicketTransaction, EventTransactionProjection, EventTicketTransaction>(q => q.Term(p => p.Event, id.HasNotNullArg(nameof(id))) && 
			q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId))),
				x => x.Transactions, x => x.Descending(p => p.Transactions.First().Date), page, take);

		public Task<Pager<EventTicketTransaction>> GetRegisteredUsersAsync(string id, string companyId,
			int page, int take)
			=> FilterNestedAsync<EventTransaction, TicketTransaction, EventTransactionProjection, EventTicketTransaction>(q =>
					q.Term(p => p.Event, id.HasNotNullArg(nameof(id))) &&
					q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId))),
				x => x.Transactions, x => x.Descending(p => p.Transactions.First().Date), page, take);

	}
}