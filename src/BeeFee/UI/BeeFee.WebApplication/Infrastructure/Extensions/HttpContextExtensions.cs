using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharpFuncExt;

namespace BeeFee.WebApplication.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static RouteInfo GetRoutingInfo(this HttpContext context)
        {
            var routingFeature = context?.Features.Get<IRoutingFeature>();
            if (routingFeature?.RouteData != null)
            {
                return new RouteInfo(routingFeature.RouteData.Values.GetArea(), routingFeature.RouteData.Values.GetController(), routingFeature.RouteData.Values.GetAction());
            }
            return null;
        }
    }
}
