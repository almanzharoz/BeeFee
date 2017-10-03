using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeeFee.ImageApp
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddImageApp(this IServiceCollection service, string settingsJsonFile)
		    => service
			    .AddSingleton(x =>
			    {
				    var settings = x.GetService<PathHandlingSettings>();
				    return new PathHandler(settings.ParentDirectory, settings.PrivateOriginalDirectory,
					    settings.PublicOriginalFolder, settings.ResizedFolder, settings.UsersDirectory,
					    settings.CompaniesDirectory, settings.UserAvatarFileName, settings.CompanyLogoFileName);
			    })
			    .AddSingleton(x => x.GetService<MemoryCacheManager>())
			    .AddSingleton(x =>
			    {
				    var setting = x.GetService<ImagesAppStartSettings>();
				    return new ImageService(x.GetService<MemoryCacheManager>(), setting.SettingsJsonFile,
					    new ImageSize(setting.UserAvatarWidth, setting.UserAvatarHeight),
					    new ImageSize(setting.CompanyLogoWidth, setting.CompanyLogoHeight),
					    new ImageSize(setting.EventImageOriginalWidth, setting.EventImageOriginalHeight), x.GetService<PathHandler>(),
					    setting.CacheTime, setting.TimeToDeleteInMinutes);
			    });
    }
}
