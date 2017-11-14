using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.ClientApp.Services
{
    public class EventService : BaseBeefeeService
    {
        public EventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
            ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
        {
        }

		public async Task<EventProjection> GetEventByUrl(string companyUrl, string url)
			=> (await FilterAsync<EventProjection>(f =>
					f.Term(p => p.Url, url.HasNotNullArg("event url")) &&
					f.HasParent<Company>(p => p.Query(q => q.Term(x => x.Url, companyUrl.HasNotNullArg("company url"))))))
				.SingleOrDefault();

		//public IReadOnlyCollection<EventSearchProjection> SearchByName(string query)
		//    => Search<Event, EventSearchProjection>(q => q
		//        .Match(m => m
		//            .Field(x => x.Name)
		//            .Query(query)));

		//public IReadOnlyCollection<EventSearchProjection> FilterEventsByNameAndCity(string query, string city)
		//    => Search<Event, EventSearchProjection>(q => q
		//        .Bool(b => b
		//            .Must(Query<Event>.Match(m => m.Field(x => x.Name).Query(query)) &&
		//                  Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)))));

		public Task<Pager<EventGridItem>> SearchEvents(string query = null, string city = null, string[] categories = null, DateTime? startDateTime = null, DateTime? endDateTime = null, decimal? maxPrice = null, int pageSize = 9, int pageIndex = 0)
        {
            List<QueryContainer> qc = new List<QueryContainer>();
            if (!string.IsNullOrEmpty(query))
            {
                qc.Add(Query<Event>.Bool(d => d.Should(r => r.Match(m => m.Field(x => x.Page.Caption).Query(query)),
                                        r => r.Match(m => m.Field(x => x.Page.Html).Query(query)))));
            }
            List<QueryContainer> qf = new List<QueryContainer>();
            qf.Add(Query<Event>.Terms(m => m.Field(x => x.State).Terms(new EEventState[] { EEventState.Close, EEventState.Open })));
            if (categories != null && categories.Any())
            {
                qf.Add(Query<Event>.Terms(m => m.Field(x => x.Category).Terms(categories)));
            }
            if (!string.IsNullOrEmpty(city))
            {
                qf.Add(Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)));
            }
            if (startDateTime.HasValue)
            {
                qf.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Start).GreaterThanOrEquals(startDateTime.Value)));
            }
            if (endDateTime.HasValue)
            {
                qf.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Finish).LessThanOrEquals(endDateTime.Value)));
            }
            List<QueryContainer> notqf = new List<QueryContainer>();
            if (maxPrice.HasValue)
            {
                qf.Add(Query<Event>.Range(m => m.Field(x => x.Prices.First().Price).LessThanOrEquals((double)maxPrice.Value)) || !Query<Event>.Exists(x => x.Field(p => p.Prices.First().Price)));
                //qf.Add(!Query<Event>.Exists(x => x.Field(p => p.Prices)));
            }
            return SearchPagerAsync<Event, EventGridItem>(q => q
                .Bool(b => b
                    .Must(qc.ToArray())
                    //.IfAny(notqf, x => x.MustNot(notqf.ToArray()))
                    .Filter(qf.ToArray())), pageIndex, pageSize, null, false);
        }


		public bool RegisterToEvent(string id, string companyId, string email, string name, string phoneNumber, Guid ticketId, string imagesUrl)
			=> (Update<RegisterToEventProjection>(f => f
				.Term(p => p.Event, id.HasNotNullArg(nameof(id))) && f.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId))) &&
					f.Nested(n => n.Path(p => p.Prices).Query(q => q.Term(t => t.Prices.First().Id, ticketId) && q.Range(r => r.Field(p => p.Prices.First().Left).GreaterThan(0.0)))),
				u => u
					.Inc(p => p.TicketsLeft, -1)
					.IncNested(p => p.Prices, p => p.Left, ticketId, -1)
					.Add(p => p.Transactions, new RegisterToEventTransaction(ticketId, DateTime.Now, new Contact(name, email, phoneNumber), 0, ETransactionType.Registrition))
				, true) > 0).IfTrue(() => AddJob(
					base.GetById<EventProjection, BaseCompanyProjection>(id, companyId).Convert(x => new CreateTicket(x.Name, name, x.Page.Date, email, imagesUrl+x.Page.Cover, x.Page.Label, Guid.NewGuid().ToString()))
					, DateTime.UtcNow));

		//TODO: public bool RegisterToEventAsync(string id, string companyId, string email, string name, string phoneNumber,
		//	Guid ticketId, string imagesUrl)
			


		//TODO сделать агргегацию посредством эластика+кеширование
		// скорее всего сделаем справочник городов
		public IReadOnlyCollection<string> GetAllCities()
            => Filter<Event, EventAddressProjection>(q => q).Select(a => a.Address.City).Distinct().OrderBy(c => c).ToArray();

    }
}