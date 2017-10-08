using System;
using BeeFee.WebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.WebApplication.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exceptionService = context.HttpContext.RequestServices.GetService<ExceptionService>();
            var result = exceptionService.Handle(context);
            if (result != null)
            {
                context.Result = result;
                context.ExceptionHandled = true;
            }
        }
    }
}
