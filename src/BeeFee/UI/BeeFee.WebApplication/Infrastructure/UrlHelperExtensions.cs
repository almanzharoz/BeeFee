using System;
using BeeFee.ImageApp;
using Microsoft.AspNetCore.Mvc;

namespace BeeFee.WebApplication.Infrastructure
{
	public static class UrlHelperExtensions
	{
		public static string GetImageUrl(this IUrlHelper helper, string filename)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/", filename);

		public static string GetImageUrl(this IUrlHelper helper, string filename, int width, int height)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/", new ImageSize(width, height).ToString(), "/", filename);
	}
}