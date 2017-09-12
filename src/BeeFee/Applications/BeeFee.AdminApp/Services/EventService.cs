using System.Collections.Generic;
using Core.ElasticSearch;
using BeeFee.AdminApp.Projections.Event;
using BeeFee.Model;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.AdminApp.Services
{
	public class EventService : BaseBeefeeService
	{
		public EventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		public EventProjection GetEvent(string id) => GetWithVersion<EventProjection>(id);

		public BaseCategoryProjection GetCategory(string id) => Get<BaseCategoryProjection>(id);

		///<exception cref="RemoveEntityException"></exception>
		public bool RemoveEvent(string id, int version)
			=> Remove<EventProjection>(id, version, true)
				.ThrowIfNot<RemoveEntityException>();

		public IReadOnlyCollection<EventProjection> SearchByName(string query) // TODO: Переделать на поиск по другому полю, т.к. Name везде Keyword
			=> Search<Event, EventProjection>(q => q
				.Match(m => m
					.Field(x => x.Name)
					.Query(query)));

		public bool SetCategoryToEvent(string eventId, string categoryId, int version)
			=> Update<EventProjection>(eventId, version,
				u => u.ChangeCategory(
					GetCategory(categoryId.HasNotNullArg("new category id")).HasNotNullArg("new category")),
				true);
	}
}