using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebApplication3.Models.Interfaces;

namespace WebApplication3.Infrastructure
{
	internal static class ModelBinderExtensions
	{
		public static IServiceCollection AddRequestModel<T>(this IServiceCollection services) where T : class
			=> services.AddScoped<T>(СreateModel<T>);

		public static IServiceCollection AddRequestModels(this IServiceCollection services)
		{
			var i = typeof(IRequestModel);
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => i.IsAssignableFrom(x)))
			{
				services.AddScoped(type, s => СreateModel(s, type));
			}
			return services;
		}

		private static T СreateModel<T>(IServiceProvider serviceProvider)
			=> (T)СreateModel(serviceProvider, typeof(T));

		public static object СreateModel(IServiceProvider serviceProvider, Type type)
		{
			var ctx = serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
			var p = new CompositeValueProvider(new List<IValueProvider>(new IValueProvider[]
			{
				new QueryStringValueProvider(BindingSource.Query, ctx.Request.Query, CultureInfo.CurrentCulture) ,
				new RouteValueProvider(BindingSource.Path, ctx.GetRouteData().Values, CultureInfo.CurrentCulture),
			}));
			var c = serviceProvider.GetService<IModelBinderFactory>();
			var modelMetadata = serviceProvider.GetService<IModelMetadataProvider>().GetMetadataForType(type);

			var factoryContext = new ModelBinderFactoryContext()
			{
				Metadata = modelMetadata,
				BindingInfo = new BindingInfo()
				{
					BinderModelName = modelMetadata.BinderModelName,
					BinderType = modelMetadata.BinderType,
					BindingSource = modelMetadata.BindingSource,
					PropertyFilterProvider = modelMetadata.PropertyFilterProvider,
				},

				// We're using the model metadata as the cache token here so that TryUpdateModelAsync calls
				// for the same model type can share a binder. This won't overlap with normal model binding
				// operations because they use the ParameterDescriptor for the token.
				CacheToken = modelMetadata,
			};

			var b = c.CreateBinder(factoryContext);
			var modelBindingContext = DefaultModelBindingContext.CreateBindingContext(
				new ActionContext(ctx, ctx.GetRouteData(), new ActionDescriptor()),
				p,
				modelMetadata,
				bindingInfo: null,
				modelName: "");
			b.BindModelAsync(modelBindingContext).Wait();

			return modelBindingContext.Model;
		}
	}
}