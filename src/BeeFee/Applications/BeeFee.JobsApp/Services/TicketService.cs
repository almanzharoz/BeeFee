﻿using System;
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
				var client = new jsreport.Client.ReportingService(_ticketSettings.Url);
				var report = await client.RenderAsync(new RenderRequest()
				{
					Template = new Template()
					{
						Recipe = Recipe.PhantomPdf,
						Engine = Engine.Handlebars,
						Content = "<html><head><meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\"></head><body><h2>Билет на {{event}}</h2><table><tr><td>Имя: {{name}}</td></tr><tr><td>Даты: {{date}}</td></tr></table></body></html>"
					},
					Data = data
				});
				using (var file = File.OpenWrite(Path.Combine(_ticketSettings.Folder, data.Filename+".pdf")))
					report.Content.CopyTo(file);

				AddJob(new SendMail("ticket@beefee.ru", data.Email, "Ваш билет", "Ваш билет", new[] { data.Filename + ".pdf" }), DateTime.UtcNow);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
