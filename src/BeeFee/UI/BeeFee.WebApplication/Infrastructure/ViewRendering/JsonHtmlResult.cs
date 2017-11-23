using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BeeFee.WebApplication.Infrastructure.ViewRendering
{
    public class JsonHtmlResult : JsonResult
    {
        private readonly AbstractHtmlJsonModel _jsonModel;
        private readonly object _htmlModel;
        private readonly string _viewPath;

        public JsonHtmlResult(AbstractHtmlJsonModel jsonModel, object htmlModel, string viewPath) : base(jsonModel)
        {
            _jsonModel = jsonModel;
            _htmlModel = htmlModel;
            _viewPath = viewPath;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.HttpContext.RequestServices;
            var viewRender = services.GetRequiredService<ViewRenderService>();
            var r = viewRender.RenderToStringAsync(_viewPath, _htmlModel);
            r.Wait();
            _jsonModel.Html = r.Result;
            Value = _jsonModel;
            var executor = services.GetRequiredService<JsonResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }
    }
}
