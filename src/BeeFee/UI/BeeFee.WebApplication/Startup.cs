using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.AdminApp;
using BeeFee.ClientApp;
using BeeFee.LoginApp;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp;
using BeeFee.WebApplication.Infrastructure;
using BeeFee.WebApplication.Infrastructure.Binders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BeeFee.WebApplication
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
			services.Configure<BeeFeeWebAppSettings>(Configuration.GetSection("Settings"));
			services.AddSingleton(cfg => cfg.GetService<IOptions<BeeFeeWebAppSettings>>().Value);

			services.AddMvc(opts => opts.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider()));

			services.AddLogging();

			services.AddDistributedMemoryCache();
			services.AddSession();

			services.AddAuthentication("MyCookieAuthenticationScheme")
				.AddCookie("MyCookieAuthenticationScheme", options =>
				{
					options.AccessDeniedPath = "/Account/AccessDenied";
					options.LoginPath = "/Account/Login";
				});

			//services.AddAuthorization(options =>
			//{
			//	//options.AddPolicy("ip-policy", policy => policy.Requirements.Add(new UserHostRequirement()));
			//	//options.AddPolicy("resource-allow-policy", x => { x.AddRequirements(new PermissionRequirement(new[] { Operations.Read })); });
			//});

			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<UserName>(x =>
			{
				var user = x.GetService<IHttpContextAccessor>()?.HttpContext?.User;
				Console.WriteLine("User: "+user?.Identity?.Name+" - "+(x.GetService<IHttpContextAccessor>() != null).ToString());
				if (user?.Identity != null && !String.IsNullOrWhiteSpace(user.FindFirst(ClaimTypes.NameIdentifier)?.Value))
					return new UserName(user.FindFirst(ClaimTypes.NameIdentifier).Value);
				return null;
			});

			services
				.AddBeefeeModel(new Uri("http://localhost:9200/"), s => s
					.AddBeefeeLoginApp()
					.AddBeefeeClientApp()
					.AddBeefeeOrganizerApp()
					.AddBeefeeAdminApp());
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

			app.UseMiddleware<NormalizeUrlMiddleware>();

			app.UseSession();

			app.UseAuthentication();


			app.UseMvc(routes =>
			{
				routes.MapRoute(name: "areaRoute",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			BeeFeeWebAppSettings.Instance = app.ApplicationServices.GetService<IOptions<BeeFeeWebAppSettings>>().Value;

			app.ApplicationServices
				.UseBeefeeModel(p => p
					.UseBeefeeLoginApp()
					.UseBeefeeClientApp()
					.UseBeefeeOrganizerApp()
					.UseBeefeeAdminApp());
		}
	}
}
