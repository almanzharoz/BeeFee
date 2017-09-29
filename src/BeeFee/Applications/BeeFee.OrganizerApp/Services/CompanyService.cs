﻿using System.Collections.Generic;
using System.Linq;
using BeeFee.Model;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
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

		public bool AddCompany(string name, string url)
			=> !ExistsByUrl<CompanyProjection>(url.IfNull(name, CommonHelper.UriTransliterate)) && Insert(new NewCompany(Get<BaseUserProjection>(User.Id), name, url), true);

		public IReadOnlyCollection<KeyValuePair<CompanyProjection, int>> GetMyCompanies()
			=> SearchWithScore<Company, CompanyProjection>(f => f.Term(p => p.Users.First().User, User.Id) || f.HasChild<Event>(c => c.ScoreMode(ChildScoreMode.Sum).Query(cq => cq.MatchAll())));

		public CompanyProjection GetCompany(string id)
			=> GetWithVersion<Company, CompanyProjection>(id, f => f.Term(p => p.Users.First().User, User.Id));

		public bool EditCompany(string id, int version, string name, string url, string email, string logo)
			=> UpdateWithQuery<CompanyProjection, EntityAccessException<Company>>(id, version,
				f => f.Term(p => p.Users.First().User, User.Id), () => new EntityAccessException<Company>(User, id),
				x => x.Update(name, url, email, logo), true);

		public bool RemoveCompany(string id, int version)
			=> Remove<CompanyProjection>(id.ThrowIfNullFluent(GetCompany, x => new EntityAccessException<Company>(User, x)), version, true);
	}
}