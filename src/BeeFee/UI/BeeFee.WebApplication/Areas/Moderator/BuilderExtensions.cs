using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.WebApplication.Infrastructure.Extensions;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Areas.Moderator
{
    public static class BuilderExtensions
    {
        public static List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> AddModeratorExceptionHandlers(this List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> handlersPipeline)
        {
            return handlersPipeline;
        }
    }
}
