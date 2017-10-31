using System;
using System.Collections.Generic;
using System.Linq;
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

        public T GetCompany<T>(string id) where T : BaseEntityWithVersion, IGetProjection, IProjection<Company>
            => GetWithVersionByIdAndQuery<Company, T>(id.HasNotNullArg("companyId"), f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id));

        public EventProjection GetEvent(string id, string company)
            => GetWithVersionByIdAndQuery<Event, EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

        public EventPreviewProjection GetPreviewEvent(string id, string company)
            => GetByIdAndQuery<Event, EventPreviewProjection, BaseCompanyProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, q => UserQuery<EventProjection>());

        /// <exception cref="AddEntityException"></exception>
        // TODO: добавить проверку 
        public string AddEvent(string companyId, string categoryId, string name, string label, string url, string email,
            EventDateTime dateTime, Address address, TicketPrice[] prices, string html, string cover)
        {
            // TODO: Проверять и вставлять одним запросом
            url.IfNull(name, CommonHelper.UriTransliterate)
                .ThrowIf(ExistsByUrl<EventProjection>, x => new ExistsUrlException<Event>(x));

            var newEvent = new NewEvent(
                companyId.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)),
                GetById<BaseUserProjection>(User.Id).HasNotNullEntity("user"),
                GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category"), name, label, url,
                dateTime, address, prices, html, email, cover);

            Insert<NewEvent, CompanyJoinProjection>(newEvent, true).ThrowIfNot<AddEntityException<Event>>();

            Insert(new NewEventTransaction(newEvent), false).ThrowIfNot<AddEntityException<EventTransaction>>(); // TODO: при такой ошибке должен быть откат

            return newEvent.Id;
        }

        private EventTransactionProjection GetEventTransactionById(string eventId, string companyId)
            => Filter<EventTransactionProjection>(q =>
                q.Term(p => p.Event, eventId.HasNotNullArg(nameof(eventId))) &&
                q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId)))).FirstOrDefault();

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
            string categoryId, TicketPrice[] prices)
            => UpdateById<EventProjection, CompanyJoinProjection>(id,
                    company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
                    x => x.Change(name, label, url, cover, email, dateTime, address,
                        GetById<BaseCategoryProjection>(categoryId).HasNotNullEntity("category"), prices), true);

        public bool UpdateEvent(string id, string company, int version, string html)
            => UpdateById<EventProjection, CompanyJoinProjection>(id,
                company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
                x => x.ChangeHtml(html), true);

        public IReadOnlyCollection<EventProjection> GetMyEvents(string companyId)
            => Filter<Event, EventProjection>(q => UserQuery<EventProjection>(x => x.HasParent<Company>(p => p.Query(pq => pq.Ids(id => id.Values(companyId.HasNotNullArg("company")))))));

        /// <summary>
        /// Отправить мероприятие на модерацию
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EntityAccessException{T}"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool ToModerate(string id, string company, int version)
            => UpdateById<EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version, x => x.ToModerate(), true);
    }
}