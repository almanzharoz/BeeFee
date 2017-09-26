using System;
using System.IO;
using System.Threading.Tasks;
using BeeFee.JobsApp.Projections;
using BeeFee.Model;
using BeeFee.Model.Jobs.Data;
using Core.ElasticSearch;
using jsreport.Types;
using Microsoft.Extensions.Logging;

namespace BeeFee.JobsApp.Services
{
    public class TicketService : BaseJobsService<CreateTicketJob, CreateTicket>
	{
		private readonly TicketServiceSettings _ticketSettings;

		public TicketService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, TicketServiceSettings ticketSettings) 
			: base(loggerFactory, settings, factory, null)
		{
			_ticketSettings = ticketSettings;
		}

		public Task<bool> CreateNextTicket()
			=> JobExecute(GetNextJob(), CreateTicket);

		public async Task CreateTicket(CreateTicket data)
		{
			try
			{
				var client = new jsreport.Client.ReportingService("http://localhost:58219");
				var report = await client.RenderAsync(new RenderRequest()
				{
					Template = new Template()
					{
						Recipe = Recipe.PhantomPdf,
						Engine = Engine.Handlebars,
						Content = "<table><tr><td>Hello, {{name}}!</td></tr><tr><td>Date: {{date}}</td></tr></table>"
					},
					Data = data
				});
				using (var file = File.OpenWrite(Path.Combine(_ticketSettings.Folder, data.Filename+".pdf")))
					report.Content.CopyTo(file);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
