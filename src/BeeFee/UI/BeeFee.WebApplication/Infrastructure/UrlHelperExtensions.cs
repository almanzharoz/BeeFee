using System;
using BeeFee.ImageApp;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Infrastructure
{
	public static class UrlHelperExtensions
	{
		public static string GetImageUrl(this IUrlHelper helper, string companyUrl, string url, string filename)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/min/", companyUrl, "/", url, "/", filename);

		public static string GetImageUrl(this IUrlHelper helper, string companyUrl, string url, string filename, int width, int height)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/min/", companyUrl, "/", url, "/", new ImageSize(width, height).ToString(), "/", filename);

		public static string GetImageUrl(this IUrlHelper helper, string companyUrl, string filename)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/public/companies/", companyUrl, "/", filename);

	}
}