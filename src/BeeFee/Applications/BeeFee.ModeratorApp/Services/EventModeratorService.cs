using System.Threading.Tasks;
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

		public EventPreviewProjection GetEvent(string id, string companyId)
			=> GetWithVersionByIdAndQuery<Event, EventPreviewProjection, BaseCompanyProjection>(id, companyId, 
				q => q.Term(t => t.State, EEventState.Moderating) && (!q.Exists(e => e.Field(p => p.Moderator)) || q.Term(p => p.Moderator, User.Id)));

		public EventModeratorProjection GetEvent(string id, string companyId, int version)
			=> GetByIdAndQuery<Event, EventModeratorProjection, BaseCompanyProjection>(id, companyId, version,
				q => q.Term(t => t.State, EEventState.Moderating) && (!q.Exists(e => e.Field(p => p.Moderator)) || q.Term(p => p.Moderator, User.Id)));

		public Task<bool> ModerateEvent(string id, string companyId, int version, string comment, bool moderated)
			=> UpdateWithVersionAsync<EventModeratorProjection, BaseCompanyProjection>(GetEvent(id, companyId, version), x => x.Moderate(comment, moderated), true);
	}
}
