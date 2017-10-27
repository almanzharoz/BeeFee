using System;
using System.IO;
using BeeFee.JobsApp;
using BeeFee.JobsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BeeFee.Model;
using BeeFee.Model.Jobs.Data;
using Microsoft.Extensions.Options;
using BuilderExtensions = BeeFee.JobsApp.BuilderExtensions;
using System.Threading;

namespace BeeFee.ShedulerApplication
{
	class Program
	{
		public static IConfigurationRoot Configuration { get; set; }
		static void Main(string[] args)
		{
			// Set up configuration sources.
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(AppContext.BaseDirectory))
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			var serviceProvider = new ServiceCollection()
					.AddBeefeeModel(new Uri("http://localhost:9200/"), s => s
						.AddBeefeeJobsApp())
					//.AddSingleton(x => new UserName(x.GetService<TestsUserService>().AddUser("user@mail.ru", "123", "user", _roles)))
					.Configure<MailServiceSettings>(Configuration.GetSection("MailSettings"))
					.Configure<TicketServiceSettings>(Configuration.GetSection("TicketSettings"))
					.AddSingleton(cfg => cfg.GetService<IOptions<MailServiceSettings>>().Value)
					.AddSingleton(cfg => cfg.GetService<IOptions<TicketServiceSettings>>().Value)
					.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseBeefeeModel(BuilderExtensions.UseBeefeeJobsApp);

			var ticketService = serviceProvider.GetService<TicketService>();
			var mailService = serviceProvider.GetService<MailService>();
			//ticketService.CreateTicket(new CreateTicket("Русское слово", "Tomorow", "ticket", "sdg@sdgfsd.dg", null, "sdghjksjdhg")).Wait();
			while (true)
			{
				ticketService.CreateNextTicket().Wait();
				mailService.SendNextMail().Wait();
				Thread.Sleep(10000);
			}
			//Console.WriteLine(service.CreateNextTicket());
		}
	}
}
