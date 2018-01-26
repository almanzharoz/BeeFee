using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using BeeFee.AdminApp;
using BeeFee.ClientApp;
using BeeFee.LoginApp;
using BeeFee.Model;
using BeeFee.Model.Projections;
using BeeFee.ModeratorApp;
using BeeFee.OrganizerApp;
using BeeFee.OrganizerApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebApplication3.Infrastructure;

namespace WebApplication3
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
            services.AddMvc();
				//.AddControllersWithRequestModel();

			services.Configure<BeeFeeWebAppSettings>(Configuration.GetSection("Settings"));
			services.AddSingleton(cfg => cfg.GetService<IOptions<BeeFeeWebAppSettings>>().Value);

			services.AddSingleton<ImagesHelper>();
			//services.AddBindedModel<UserModel>();

			services.AddLogging();
			//services.AddMemoryCache();
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.Cookie.Name = ".Session";
				options.IdleTimeout = TimeSpan.FromDays(365);
				options.Cookie.HttpOnly = true;
			});

			services.AddAuthentication("MyCookieAuthenticationScheme")
				.AddCookie("MyCookieAuthenticationScheme", options =>
				{
					options.AccessDeniedPath = "/Account/AccessDenied";
					options.LoginPath = "/Account/Login";
				});

			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<UserName>(x =>
			{
				var user = x.GetService<IHttpContextAccessor>()?.HttpContext?.User;
				Console.WriteLine("User: " + user?.Identity?.Name + " - " + (x.GetService<IHttpContextAccessor>() != null).ToString());
				if (user?.Identity != null && !String.IsNullOrWhiteSpace(user.FindFirst(ClaimTypes.NameIdentifier)?.Value))
					return new UserName(user.FindFirst(ClaimTypes.NameIdentifier).Value);
				return null;
			});

			services.AddSingleton(x => new ImagesService(x.GetService<BeeFeeWebAppSettings>().ImagesUrl));
			services.AddRequestModels();

			//services.AddScoped(ModelBinderExtensions.СreateModel<EventRequestModel>);

			services
				.AddBeefeeModel(new Uri("http://localhost:9200/"), s => s
					.AddBeefeeLoginApp()
					.AddBeefeeClientApp()
					.AddBeefeeOrganizerApp()
					.AddBeefeeAdminApp()
					.AddBeefeeModeratorApp());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

			app.UseSession();

			app.UseAuthentication();

			var supportedCultures = new[] { new CultureInfo("ru-RU") };
			app.UseRequestLocalization(new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("ru-RU"),
				SupportedCultures = supportedCultures,
				SupportedUICultures = supportedCultures
			});

			app.UseMvc(routes =>
            {
				routes.MapRoute(
					name: "home",
					template: "", defaults: new {controller="Events", action="Index"});

				routes.MapRoute(name: "areaRoute",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}/{parentid?}");

				routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}/{parentid?}");
            });

			app.ApplicationServices
				.UseBeefeeModel(p => p
					.UseBeefeeLoginApp()
					.UseBeefeeClientApp()
					.UseBeefeeOrganizerApp()
					.UseBeefeeAdminApp()
					.UseBeefeeModeratorApp());
		}
    }
}
