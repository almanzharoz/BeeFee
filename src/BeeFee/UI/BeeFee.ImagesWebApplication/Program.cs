using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BeeFee.ImagesWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
                .UseStartup<Startup>()
                .Build();
    }

	//TODO: Для работа загрузки файлов на удаленный сервер в web.config
	//<security>
	//<requestFiltering>
	//<!-- Measured in Bytes -->
	//<requestLimits maxAllowedContentLength = "1073741824" />  < !--1 GB-->
	//</requestFiltering>
	//</security>

}
