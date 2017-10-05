using System.IO;
using BeeFee.ImageApp;
using BeeFee.ImageApp.Embed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace BeeFee.ImagesWebApplication
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
	        services.Configure<ImagesAppStartSettings>(Configuration.GetSection("ImageAppSettings"));
	        services.AddSingleton(cfg => cfg.GetService<IOptions<ImagesAppStartSettings>>().Value);

	        services.Configure<PathHandlingSettings>(Configuration.GetSection("PathHandlingSettings"));
	        services.AddSingleton(cfg => cfg.GetService<IOptions<PathHandlingSettings>>().Value);

			services.AddMvc();
            services.AddMemoryCache();
            services.AddCors();
			services.AddImageApp();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

	        //app.UseFileServer(app.ApplicationServices.GetService<IOptions<ImagesWebSettings>>().Value.PublicImagesFolder);

	        app.UseFileServer(new FileServerOptions()
	        {
		        FileProvider = new PhysicalFileProvider(
			        Path.Combine(Directory.GetCurrentDirectory(), @"images")),
		        RequestPath = new PathString(""),
		        EnableDirectoryBrowsing = false
	        });

			app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader());
            app.UseMvc();
        }
    }
}
