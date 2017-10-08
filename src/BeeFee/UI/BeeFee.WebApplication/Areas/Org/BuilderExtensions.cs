using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.WebApplication.Infrastructure.Extensions;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Areas.Org
{
    public static class BuilderExtensions
    {
        public static List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> AddOrgExceptionHandlers(this List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> handlersPipeline)
        {
            handlersPipeline.Add(("Org", new List<Func<RouteInfo, Exception, IActionResult>>(){
                ( info, exception) =>
                {
                    if (info.Is("Event", "Remove"))
                    {
                        return ExceptionService.StandardErrorPage("Ошибка удаления мероприятия");
                    }
                    return null;
                }
            }));
            return handlersPipeline;
        }
    }
}
