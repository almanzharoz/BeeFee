using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApplication3
{
	public static class ModelBinderExtensions
	{
		public static IServiceCollection AddBindedModel<T>(this IServiceCollection services) where T : class
			=> services.AddTransient<T>(СreateModel<T>);

		private static T СreateModel<T>(IServiceProvider serviceProvider)
		{
			var ctx = serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
			var p = new CompositeValueProvider(new List<IValueProvider>(new IValueProvider[]
			{
				new QueryStringValueProvider(BindingSource.Query, ctx.Request.Query, CultureInfo.CurrentCulture) ,
				new RouteValueProvider(BindingSource.Path, ctx.GetRouteData().Values, CultureInfo.CurrentCulture),
			}));
			var c = serviceProvider.GetService<IModelBinderFactory>();
			var modelMetadata = serviceProvider.GetService<IModelMetadataProvider>().GetMetadataForType(typeof(T));

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
			
			return (T)modelBindingContext.Model;
		}

		internal static object СreateModel(IServiceProvider serviceProvider, Type type)
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

		public static IMvcBuilder AddControllersAsServices(this IMvcBuilder builder)
		{
			builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ModelBasedControllerActivator>());

			return builder;
		}
	}

	public class ModelBasedControllerActivator : IControllerActivator
	{
		public object Create(ControllerContext actionContext)
		{
			var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
			var provider = actionContext.HttpContext.RequestServices;
			var modelType = controllerType.GetConstructors().FirstOrDefault()?.GetParameters().FirstOrDefault(x => x.Name == "model")?.ParameterType;
			if (modelType == null)
				return ActivatorUtilities.CreateInstance(provider, controllerType);
			var model = ModelBinderExtensions.СreateModel(provider, modelType);
			return ActivatorUtilities.CreateInstance(provider, controllerType, model);
		}

		public virtual void Release(ControllerContext context, object controller)
		{
		}
	}
}