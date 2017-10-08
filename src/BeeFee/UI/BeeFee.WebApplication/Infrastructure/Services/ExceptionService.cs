using System;
using System.Collections.Generic;
using System.Net;
using BeeFee.WebApplication.Infrastructure.Extensions;
using BeeFee.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BeeFee.WebApplication.Infrastructure.Services
{
    public class ExceptionService
    {
        private readonly List<(string Area, List<Func<RouteInfo, Exception, IActionResult>> Handlers)> _handlersPipeline;

        public ExceptionService(Func<List<(string, List<Func<RouteInfo, Exception, IActionResult>>)>, List<(string, List<Func<RouteInfo, Exception, IActionResult>>)>> handlersPipelineBuilder)
        {
            _handlersPipeline = handlersPipelineBuilder(new List<(string, List<Func<RouteInfo, Exception, IActionResult>>)>());
            _handlersPipeline.Add(("", new List<Func<RouteInfo, Exception, IActionResult>>() {
                (i, c) => StandardErrorPage("Возникла непредвиденная ошибка")
            }));
        }

        public IActionResult Handle(ExceptionContext context)
        {
            IActionResult result = null;
            for (int i = 0; i < _handlersPipeline.Count; i++)
            {
                var routeInfo = context.HttpContext.GetRoutingInfo();
                if (routeInfo != null && (_handlersPipeline[i].Area == null ||
                    routeInfo.Area.Equals(_handlersPipeline[i].Area, StringComparison.InvariantCultureIgnoreCase)))
                {
                    for (int j = 0; j < _handlersPipeline.Count; i++)
                    {
                        result = _handlersPipeline[i].Handlers[j](routeInfo, context.Exception);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        public static IActionResult StandardErrorPage(string message) =>
            new ViewResult()
            {
                ViewName = "Error",
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "text/html",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new ErrorPageModel(message)
                }
            };
    }
}
