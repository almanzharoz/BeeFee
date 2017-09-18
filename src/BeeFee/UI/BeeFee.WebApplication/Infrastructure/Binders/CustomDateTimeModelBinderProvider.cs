using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace BeeFee.WebApplication.Infrastructure.Binders
{
    public class CustomDateTimeModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder =
            new CustomDateTimeModelBinder(new SimpleTypeModelBinder(typeof(DateTime)));

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(DateTime)|| context.Metadata.ModelType == typeof(DateTime?) ? binder : null;
        }
    }
}
