using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using BeeFee.AdminApp;
using BeeFee.ClientApp;
using BeeFee.LoginApp;
using BeeFee.Model;
using BeeFee.Model.Projections;
using BeeFee.OrganizerApp;
using BeeFee.WebApplication.Infrastructure;
using BeeFee.WebApplication.Infrastructure.Binders;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using BeeFee.ModeratorApp;
using BeeFee.WebApplication.Infrastructure.Filters;
using BeeFee.WebApplication.Infrastructure.Middleware;
using Microsoft.AspNetCore.Diagnostics;

namespace BeeFee.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<BeeFeeWebAppSettings>(Configuration.GetSection("Settings"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<BeeFeeWebAppSettings>>().Value);

            services.AddMvc(opts =>
            {
                opts.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
                if (!CurrentEnvironment.IsDevelopment())
                    opts.Filters.Add(new GlobalExceptionFilter());
            });

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
                Console.WriteLine("User: " + user?.Identity?.Name + " - " + (x.GetService<IHttpContextAccessor>() != null).ToString());
                if (user?.Identity != null && !String.IsNullOrWhiteSpace(user.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                    return new UserName(user.FindFirst(ClaimTypes.NameIdentifier).Value);
                return null;
            });


            services.AddWebApp()
                .AddBeefeeModel(new Uri("http://localhost:9200/"), s => s
                    .AddBeefeeLoginApp()
                    .AddBeefeeClientApp()
                    .AddBeefeeOrganizerApp()
                    .AddBeefeeAdminApp()
                    .AddBeefeeModeratorApp());

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString(""),
                EnableDirectoryBrowsing = false
            });

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
                    .UseBeefeeAdminApp()
                    .UseBeefeeModeratorApp());
        }
    }
}
