using BeeFee.ImageApp2.Caching;
using BeeFee.ImageApp2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.ImageApp2
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddImageApp(this IServiceCollection service)
			=> service
				.AddSingleton<MemoryCacheManager>()
				.AddSingleton<ImageService>();
	}
}