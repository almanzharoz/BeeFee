using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeeFee.WebApplication.Infrastructure.Middleware
{
	public class NormalizeUrlMiddleware
	{
		private readonly RequestDelegate _next;

		public NormalizeUrlMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext context)
		{
			if (context.Request.Method.ToUpper() == "GET" && !String.IsNullOrEmpty(context.Request.Path.Value/*.ToLower()*/.Trim().TrimEnd('/')) && !context.Request.Path.Value.Equals(context.Request.Path.Value/*.ToLower()*/.Trim().TrimEnd('/')))
			{
				context.Response.Redirect(context.Request.Path.Value/*.ToLower()*/.Trim().TrimEnd('/') + context.Request.QueryString.ToUriComponent().TrimEnd('?'), true);
				return Task.FromResult(0);
			}
			if (context.Request.Method.ToUpper() == "GET" && !String.IsNullOrEmpty(context.Request.QueryString.ToUriComponent()) && !context.Request.QueryString.ToUriComponent().Equals(context.Request.QueryString.ToUriComponent().TrimEnd('?')))
			{
				context.Response.Redirect(context.Request.Path.Value/*.ToLower()*/.Trim().TrimEnd('/') + context.Request.QueryString.ToUriComponent().TrimEnd('?'), true);
				return Task.FromResult(0);
			}
			return _next.Invoke(context);
		}
	}
}