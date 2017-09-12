using System.Collections.Generic;
using Core.ElasticSearch;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections.Event;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Services
{
	public class EventService : BaseBeefeeService
	{
		public EventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public EventProjection GetEvent(string id)
			=> GetWithVersion<Event, EventProjection>(id, f => UserQuery<EventProjection>(null));

		/// <exception cref="AddEntityException"></exception>
		// TODO: добавить проверку 
		public bool AddEvent(string categoryId, string name, string url, string cover, EEventType type,
			EventDateTime dateTime,
			Address address, TicketPrice[] prices, string html)
			=> Insert(
				new NewEvent(Get<BaseUserProjection>(User.Id), Get<BaseCategoryProjection>(categoryId), name, url, cover, type,
					dateTime,
					address, prices, html), true);

		///<exception cref="RemoveEntityException"></exception>
		public bool RemoveEvent(string id, int version)
			// TODO: Добавить проверку пользователя
			=> Remove<EventProjection>(id, version, true);

		///<exception cref="UpdateEntityException"></exception>
		public bool UpdateEvent(string id, int version, string name, string url, string cover, EventDateTime dateTime, Address address, EEventType type,
			string categoryId, TicketPrice[] prices, string html)
			=> Update<EventProjection>(id, version, x => x.Change(name, url, cover, dateTime, address, type, Get<BaseCategoryProjection>(categoryId.HasNotNullArg("category")), prices, html), true)
				.ThrowIfNot<UpdateEntityException>();

		public IReadOnlyCollection<EventProjection> GetMyEvents() 
			=> Filter<Event, EventProjection>(q => UserQuery<EventProjection>(null));
	}
}