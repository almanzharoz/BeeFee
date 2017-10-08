using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.WebApplication.Infrastructure.Extensions;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Areas.Admin
{
    public static class BuilderExtensions
    {
        public static List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> AddAdminExceptionHandlers(this List<(string area, List<Func<RouteInfo, Exception, IActionResult>> handlers)> handlersPipeline)
        {
            return handlersPipeline;
        }
    }
}
