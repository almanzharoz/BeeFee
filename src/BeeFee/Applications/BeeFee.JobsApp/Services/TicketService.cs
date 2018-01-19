using System;
using System.IO;
using System.Linq;
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
				var @event = GetById<EventTicketProjection>(data.EventTransactionId);
				var price = @event.Prices.First(/*x => x.Id == data.PriceId*/);
				var client = new jsreport.Client.ReportingService(_ticketSettings.Url);
				var report = await client.RenderAsync(new RenderRequest()
				{
					Template = new Template()
					{
						Recipe = Recipe.PhantomPdf,
						Engine = Engine.Handlebars,
						Content = price.TicketTemplate
					},
					Data = data /*TODO: Сделать полноценную ViewModel */
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
