using System;

namespace WebApplication3.Infrastructure
{
    public class ImagesHelper
	{
		private readonly BeeFeeWebAppSettings _settings;

		public ImagesHelper(BeeFeeWebAppSettings settings)
			=> _settings = settings;

		public string GetImageUrl(string companyUrl, string url, string filename)
			=> String.Concat(_settings.ImagesUrl, "/", companyUrl, "/", url, "/", filename);

		public string GetImageUrl(string companyUrl, string url, string filename, int width, int height)
			=> String.Concat(_settings.ImagesUrl, "/", companyUrl, "/", url, "/", width, "x", height, "/", filename);

		public string GetImageUrl(string companyUrl, string filename)
			=> String.Concat(_settings.ImagesUrl, "/", companyUrl, "/", filename);

		public string GetImageUrl() => _settings.ImagesUrl;

	}
}
