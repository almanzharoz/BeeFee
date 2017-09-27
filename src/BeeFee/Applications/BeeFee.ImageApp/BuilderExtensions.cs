using System;
using System.Collections.Generic;
using System.Text;
using BeeFee.ImageApp.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeeFee.ImageApp
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddImageApp(this IServiceCollection service, string settingsJsonFile)
            => service.AddSingleton(x =>
            {
                var setting = x.GetService<ImagesAppStartSettings>();
                return new ImageApp.Services.ImageService(setting.ImagesFolder, setting.PublicOriginalFolder,
                    setting.PrivateOriginalFolder, setting.ResizedFolder,
                    new ImageSize(setting.MaxOriginalWidth, setting.MaxOriginalHeight), settingsJsonFile,
                    setting.RemoveImageAvailabilityTime);
            }).AddSingleton(cfg => cfg.GetService<MemoryCacheManager>());
    }
}
