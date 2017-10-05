using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.ImageApp
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddImageApp(this IServiceCollection service)
		    => service
			    .AddSingleton(x =>
			    {
				    var settings = x.GetService<PathHandlingSettings>();
				    return new PathHandler(settings.ParentDirectory, settings.PrivateOriginalDirectory,
					    settings.PublicOriginalFolder, settings.ResizedFolder, settings.UsersDirectory,
					    settings.CompaniesDirectory, settings.UserAvatarFileName, settings.CompanyLogoFileName);
			    })
				.AddSingleton(x => new MemoryCacheManager(x.GetService<IMemoryCache>()))
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
