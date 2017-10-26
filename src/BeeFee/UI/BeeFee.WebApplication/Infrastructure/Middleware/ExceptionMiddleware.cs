using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BeeFee.WebApplication.Infrastructure.Middleware
{
	public class ExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeveloperExceptionPageMiddleware"/> class
		/// </summary>
		/// <param name="next"></param>
		/// <param name="loggerFactory"></param>
		public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			if (next == null)
				throw new ArgumentNullException(nameof(next));

			_next = next;
			_logger = loggerFactory.CreateLogger<DeveloperExceptionPageMiddleware>();
		}

		/// <summary>
		/// Process an individual request.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				//_logger.UnhandledException(ex);

				if (context.Response.HasStarted)
				{
					//_logger.ResponseStartedErrorPageMiddleware();
					throw;
				}

				try
				{
					context.Response.Clear();
					context.Response.StatusCode = 500;

					//await DisplayException(context, ex);

					return;
				}
				catch (Exception ex2)
				{
					// If there's a Exception while generating the error page, re-throw the original exception.
					//_logger.DisplayErrorPageException(ex2);
				}
				throw;
			}
		}

		// Assumes the response headers have not been sent.  If they have, still attempt to write to the body.
		//private Task DisplayException(HttpContext context, Exception ex)
		//{
		//	var compilationException = ex as ICompilationException;
		//	if (compilationException != null)
		//	{
		//		return DisplayCompilationException(context, compilationException);
		//	}

		//	return DisplayRuntimeException(context, ex);
		//}

		//private Task DisplayCompilationException(
		//	HttpContext context,
		//	ICompilationException compilationException)
		//{
		//	var model = new CompilationErrorPageModel
		//	{
		//		Options = _options,
		//	};

		//	var errorPage = new CompilationErrorPage
		//	{
		//		Model = model
		//	};

		//	if (compilationException.CompilationFailures == null)
		//	{
		//		return errorPage.ExecuteAsync(context);
		//	}

		//	foreach (var compilationFailure in compilationException.CompilationFailures)
		//	{
		//		if (compilationFailure == null)
		//		{
		//			continue;
		//		}

		//		var stackFrames = new List<StackFrameSourceCodeInfo>();
		//		var exceptionDetails = new ExceptionDetails
		//		{
		//			StackFrames = stackFrames,
		//			ErrorMessage = compilationFailure.FailureSummary,
		//		};
		//		model.ErrorDetails.Add(exceptionDetails);
		//		model.CompiledContent.Add(compilationFailure.CompiledContent);

		//		if (compilationFailure.Messages == null)
		//		{
		//			continue;
		//		}

		//		var sourceLines = compilationFailure
		//				.SourceFileContent?
		//				.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

		//		foreach (var item in compilationFailure.Messages)
		//		{
		//			if (item == null)
		//			{
		//				continue;
		//			}

		//			var frame = new StackFrameSourceCodeInfo
		//			{
		//				File = compilationFailure.SourceFilePath,
		//				Line = item.StartLine,
		//				Function = string.Empty
		//			};

		//			if (sourceLines != null)
		//			{
		//				_exceptionDetailsProvider.ReadFrameContent(frame, sourceLines, item.StartLine, item.EndLine);
		//			}

		//			frame.ErrorDetails = item.Message;

		//			stackFrames.Add(frame);
		//		}
		//	}

		//	return errorPage.ExecuteAsync(context);
		//}

		//private Task DisplayRuntimeException(HttpContext context, Exception ex)
		//{
		//	var request = context.Request;

		//	var model = new ErrorPageModel
		//	{
		//		Options = _options,
		//		ErrorDetails = _exceptionDetailsProvider.GetDetails(ex),
		//		Query = request.Query,
		//		Cookies = request.Cookies,
		//		Headers = request.Headers
		//	};

		//	var errorPage = new ErrorPage(model);
		//	return errorPage.ExecuteAsync(context);
		//}
	}
}