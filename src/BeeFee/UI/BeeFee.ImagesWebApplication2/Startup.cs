using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp2;
using BeeFee.ImageApp2.Embed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeeFee.ImagesWebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.Configure<ImageAppStartSettings>(Configuration.GetSection("ImagesServerSettings"));
			services.AddSingleton(cfg => cfg.GetService<IOptions<ImageAppStartSettings>>().Value);

			services.AddMvc();
			services.AddCors(x => x.AddPolicy("cors", z => z.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build()));

			services.AddImageApp();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseFileServer(new FileServerOptions()
			{
				FileProvider = new PhysicalFileProvider(app.ApplicationServices.GetService<IOptions<ImageAppStartSettings>>().Value.PreviewDirectory),
				RequestPath = new PathString(""),
				EnableDirectoryBrowsing = false
			});

			app.Use((x, n) =>
			{
				Console.WriteLine(
					$"Request host: {x.Connection.RemoteIpAddress}, method: {x.Request.Method}, url: {x.Request.Path}");
				return n();
			});

			app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());
			app.UseMvc();
        }
    }
}
