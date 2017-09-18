using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BeeFee.WebApplication.Infrastructure.Binders
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;
        public CustomDateTimeModelBinder(IModelBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var datetimePartValues = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            if (datetimePartValues == ValueProviderResult.None)
                return fallbackBinder.BindModelAsync(bindingContext);

            string valStr = datetimePartValues.FirstValue;

            DateTime outDateValue;
            if (!DateTime.TryParseExact(valStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out outDateValue) && !DateTime.TryParseExact(valStr, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out outDateValue))
                DateTime.TryParse(valStr, out outDateValue);

            // устанавливаем результат привязки
            bindingContext.Result = ModelBindingResult.Success(outDateValue);
            return Task.CompletedTask;
        }
    }
}
