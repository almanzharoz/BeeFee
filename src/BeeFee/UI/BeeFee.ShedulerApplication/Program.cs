using System;
using System.Diagnostics;
using System.IO;
using BeeFee.JobsApp;
using BeeFee.JobsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BeeFee.Model;
using Microsoft.Extensions.Options;
using BuilderExtensions = BeeFee.JobsApp.BuilderExtensions;

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
					.AddSingleton(cfg => cfg.GetService<IOptions<MailServiceSettings>>().Value)
					.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseBeefeeModel(BuilderExtensions.UseBeefeeJobsApp);

			var service = serviceProvider.GetService<MailService>();
			Console.WriteLine(service.SendNextMail());
		}
	}
}
