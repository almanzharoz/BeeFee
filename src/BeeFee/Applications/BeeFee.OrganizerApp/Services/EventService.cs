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
			=> GetWithVersion<Company, T>(id, f => Query<Company>.Term(p => p.Users.First().User, User.HasNotNullArg(x => x.Id, "user").Id));

		public EventProjection GetEvent(string id, string company)
			=> GetWithVersion<Event, EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)).Id, UserQuery<EventProjection>);

		/// <exception cref="AddEntityException"></exception>
		// TODO: добавить проверку 
		public bool AddEvent(string companyId, string categoryId, string name, string label, string url, string email, 
			EventDateTime dateTime, Address address, TicketPrice[] prices, string html, string imagesKey)
		{
			var newEvent = new NewEvent(
				companyId.ThrowIfNull(GetCompany<CompanyJoinProjection>, x => new EntityAccessException<Company>(User, x)),
				Get<BaseUserProjection>(User.Id).HasNotNullEntity("user"),
				Get<BaseCategoryProjection>(categoryId), name, label, url,
				dateTime, address, prices, html, email);
			return !ExistsByUrl<EventProjection>(url.IfNull(name, CommonHelper.UriTransliterate)) &&
				Insert<NewEvent, CompanyJoinProjection>(
					newEvent, true) && Insert(new NewEventTransaction(newEvent), false);
		}

		private EventTransactionProjection GetEventTransactionById(string eventId, string companyId)
			=> Filter<EventTransactionProjection>(q =>
				q.Term(p => p.Event, eventId.HasNotNullArg(nameof(eventId))) &&
				q.Term(p => p.Company, companyId.HasNotNullArg(nameof(companyId)))).FirstOrDefault();

		///<exception cref="RemoveEntityException"></exception>
		public bool RemoveEvent(string id, string company, int version)
			// TODO: Добавить проверку статуса редактирования
			=> Remove<EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version, true)
			&& Remove(GetEventTransactionById(id, company), false);

		///<exception cref="UpdateEntityException"></exception>
		public bool UpdateEvent(string id, string company, int version, string name, string label, string url, string cover, string email,
			EventDateTime dateTime, Address address, 
			string categoryId, TicketPrice[] prices, string html)
			=> Update<EventProjection, CompanyJoinProjection>(id,
					company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version,
					x => x.Change(name, label, url, cover, email, dateTime, address, 
						Get<BaseCategoryProjection>(categoryId).HasNotNullEntity("category"), prices, html), true);

		public IReadOnlyCollection<EventProjection> GetMyEvents(string companyId) 
			=> Filter<Event, EventProjection>(x => UserQuery<EventProjection>(x.ParentId(p=>p.Id(companyId.HasNotNullArg("company")))));

		public bool ToModerate(string id, string company, int version)
			=> Update<EventProjection, CompanyJoinProjection>(id, company.ThrowIfNull(GetCompany<CompanyProjection>, x => new EntityAccessException<Company>(User, x)).Id, version, x => x.ToModerate(), true);
	}
}