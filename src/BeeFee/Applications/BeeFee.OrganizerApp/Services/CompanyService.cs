using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp.Projections;
using BeeFee.OrganizerApp.Projections.Company;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Services
{
	public class CompanyService : BaseBeefeeService
	{
		public CompanyService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public CompanyProjection AddCompany(string name, string url, string email, string logo)
			=> url.IfNull(name, CommonHelper.UriTransliterate).IfNot(ExistsByUrl<CompanyProjection>,
				x => InsertWithVersion<NewCompany, CompanyProjection>(new NewCompany(GetById<BaseUserProjection>(User.Id), name, x, email, logo)), 
				x => null);

		public Task<CompanyProjection> AddCompanyAsync(string name, string url, string email, string logo)
			=> url.IfNull(name, CommonHelper.UriTransliterate)
				.IfNot(ExistsByUrl<CompanyProjection>,
					x => InsertWithVersionAsync<NewCompany, CompanyProjection>(new NewCompany(GetById<BaseUserProjection>(User.Id), name, x, email, logo)),
					x => Task.FromResult(default(CompanyProjection)));

		public bool StartOrg()
			=> UpdateById<UserUpdateProjection>(User.Id, u => u.StartOrg(), true);

		public Task<bool> StartOrgAsync()
			=> UpdateByIdAsync<UserUpdateProjection>(User.Id, u => u.StartOrg(), true);

		public IReadOnlyCollection<KeyValuePair<CompanyProjection, int>> GetMyCompanies()
			=> SearchWithScore<Company, CompanyProjection>(f =>
				f.Bool(b => b.Must(m => m.Term(p => p.Users.First().User, User.Id, 0.0))
					.Should(s => s.HasChild<Event>(c => c.Query(q => q.MatchAll()).ScoreMode(ChildScoreMode.Sum)))), x => x.Ascending(p => p.Name));

		public Task<KeyValuePair<CompanyProjection, int>[]> GetMyCompaniesAsync()
			=> SearchWithScoreAsync<Company, CompanyProjection>(f =>
				f.Bool(b => b.Must(m => m.Term(p => p.Users.First().User, User.Id, 0.0))
					.Should(s => s.HasChild<Event>(c => c.Query(q => q.MatchAll()).ScoreMode(ChildScoreMode.Sum)))), x => x.Ascending(p=>p.Name));

		public CompanyProjection GetCompany(string id)
			=> id.IfNotNull(
				x => GetWithVersionByIdAndQuery<Company, CompanyProjection>(x, f => f.Term(p => p.Users.First().User, User.Id)),
				() => Filter<CompanyProjection>(f =>
						f.Term(p => p.Users.First().User, User.Id) && f.Term(p => p.Users.First().Role, ECompanyUserRole.Owner))
					.FirstOrDefault());

		public Task<CompanyProjection> GetCompanyAsync(string id)
			=> id.IfNotNull(
				x => GetWithVersionByIdAndQueryAsync<Company, CompanyProjection>(x, f => f.Term(p => p.Users.First().User, User.Id)),
				() => FilterFirstAsync<CompanyProjection>(f =>
						f.Term(p => p.Users.First().User, User.Id) && f.Term(p => p.Users.First().Role, ECompanyUserRole.Owner)));

		public bool EditCompany(string id, int version, string name, string url, string email, string logo)
			=> UpdateByIdAndQuery<CompanyProjection, EntityAccessException<Company>>(id, version,
				f => f.Term(p => p.Users.First().User, User.Id), () => new EntityAccessException<Company>(User, id),
				x => x.Update(name, url, email, logo), true);

		public Task<bool> EditCompanyAsync(string id, int version, string name, string url, string email, string logo)
			=> UpdateByIdAndQueryAsync<CompanyProjection, EntityAccessException<Company>>(id, version,
				f => f.Term(p => p.Users.First().User, User.Id), () => new EntityAccessException<Company>(User, id),
				x => x.Update(name, url, email, logo), true);

		public bool RemoveCompany(string id, int version)
			=> Remove<CompanyProjection>(id.ThrowIfNullFluent(GetCompany, x => new EntityAccessException<Company>(User, x)), version, true);

	}
}