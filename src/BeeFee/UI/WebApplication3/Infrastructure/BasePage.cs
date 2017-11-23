using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using SharpFuncExt;

namespace WebApplication3.Infrastructure
{
    public abstract class BasePage<TModel> : RazorPage<TModel>
        where TModel : class
    {
        private BaseModel _baseModel;

        private BaseModel GetBaseModel() => _baseModel.IfNull(() =>
        {
            _baseModel = (BaseModel)ModelBinderExtensions.СreateModel(Context.RequestServices, typeof(BaseModel));
            return _baseModel;
        });

        public string Id => GetBaseModel().Id;
        public string ParentId => GetBaseModel().ParentId;
        public int Version => GetBaseModel().Version;

        private class BaseModel
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
            public int Version { get; set; }
        }
    }
}
