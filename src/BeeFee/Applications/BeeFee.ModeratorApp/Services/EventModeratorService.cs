using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.ModeratorApp.Projections;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.ModeratorApp.Services
{
    public class EventModeratorService : BaseBeefeeService
    {
		public EventModeratorService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public Pager<EventModeratorGridItem> GetEvents(int page, int take)
			=> FilterPager<Event, EventModeratorGridItem>(q => q.Term(p => p.State, EEventState.Moderating), page, take,
				s => s.Ascending(p => p.DateTime.Start));

		public EventModeratorProjection GetEvent(string id, string companyId)
			=> GetWithVersionByIdAndQuery<Event, EventModeratorProjection, BaseCompanyProjection>(id, companyId, 
				q => q.Term(t => t.State, EEventState.Moderating) && (!q.Exists(e => e.Field(p => p.Moderator)) || q.Term(p => p.Moderator, User.Id)));

		public EventModeratorProjection GetEvent(string id, string companyId, int version)
			=> GetByIdAndQuery<Event, EventModeratorProjection, BaseCompanyProjection>(id, companyId, version,
				q => q.Term(t => t.State, EEventState.Moderating) && (!q.Exists(e => e.Field(p => p.Moderator)) || q.Term(p => p.Moderator, User.Id)));

		public bool ModerateEvent(string id, string companyId, int version, string title, string caption, string label,
			Address address, string categoryId, string html, bool moderated)
			=> UpdateWithVersion<EventModeratorProjection, BaseCompanyProjection>(GetEvent(id, companyId, version), x => x.Change(title, label, caption, address, GetById<BaseCategoryProjection>(categoryId, false).HasNotNullArg("category"), html).ChangeType(moderated), true);

		public bool ModerateEvent(string id, string companyId, int version, bool moderated)
			=> UpdateWithVersion<EventModeratorProjection, BaseCompanyProjection>(GetEvent(id, companyId, version), x => x.ChangeType(moderated), true);
	}
}
