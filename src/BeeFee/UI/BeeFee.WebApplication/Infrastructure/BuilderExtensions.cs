using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.Model.Exceptions;
using BeeFee.OrganizerApp.Projections.Company;
using BeeFee.WebApplication.Areas.Admin;
using BeeFee.WebApplication.Areas.Moderator;
using BeeFee.WebApplication.Areas.Org;
using BeeFee.WebApplication.Infrastructure.Extensions;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.WebApplication.Infrastructure
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddWebApp(this IServiceCollection services)
        {
            return services
                .AddScoped<ViewRenderService>()
                .AddScoped<EventUIService>()
                .AddScoped<ExceptionService>(provider => new ExceptionService(s => s
                .AddCommonExceptionHandlers()
                .AddAdminExceptionHandlers()
                .AddModeratorExceptionHandlers()
                .AddOrgExceptionHandlers()));
        }

        public static List<(string, List<Func<RouteInfo, Exception, IActionResult>>)> AddCommonExceptionHandlers(this List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> handlersPipeline)
        {
            handlersPipeline.Add((null, new List<Func<RouteInfo, Exception, IActionResult>>(){
                ( info, exception) =>
                {
                    if (exception.GetType().Equals(typeof(EntityAccessException<CompanyProjection>)))
                    {
                        return ExceptionService.StandardErrorPage("Ошибка доступа к компании");
                    }
                    return null;
                }
            }));
            return handlersPipeline;
        }
    }
}