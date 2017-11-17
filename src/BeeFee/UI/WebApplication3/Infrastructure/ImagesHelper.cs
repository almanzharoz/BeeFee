using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Infrastructure
{
    public class ImagesHelper
	{
		private readonly BeeFeeWebAppSettings _settings;

		public ImagesHelper(BeeFeeWebAppSettings settings)
			=> _settings = settings;

		public string GetImageUrl(string companyUrl, string url, string filename)
			=> String.Concat(_settings.ImagesUrl, "/min/", companyUrl, "/", url, "/", filename);

		public string GetImageUrl(string companyUrl, string url, string filename, int width, int height)
			=> String.Concat(_settings.ImagesUrl, "/min/", companyUrl, "/", url, "/", width, "x", height, "/", filename);

		public string GetImageUrl(string companyUrl, string filename)
			=> String.Concat(_settings.ImagesUrl, "/public/companies/", companyUrl, "/", filename);

	}
}
