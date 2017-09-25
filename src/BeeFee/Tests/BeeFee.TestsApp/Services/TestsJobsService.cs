using System;
using BeeFee.Model;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Projections;
using BeeFee.Model.Projections.Jobs;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;

namespace BeeFee.TestsApp.Services
{
	public class TestsJobsService : BaseBeefeeService
	{
		public TestsJobsService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public bool AddSendMailJob(SendMail data, DateTime start)
			=> Insert(new NewJob<SendMail>(data, start), true);
	}
}