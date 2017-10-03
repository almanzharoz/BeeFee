using System;

namespace BeeFee.ImageApp.Embed
{
    public class ImagesAppStartSettings
    {
		public string SettingsJsonFile { get; set; }
		//public ImageSize UserAvatarSize { get; set; }
		public int UserAvatarWidth { get; set; }
		public int UserAvatarHeight { get; set; }
		//public ImageSize CompanyLogoSize { get; set; }
		public int CompanyLogoWidth { get; set; }
		public int CompanyLogoHeight { get; set; }
		//public ImageSize EventImageOriginalSize { get; set; }
		public int EventImageOriginalWidth { get; set; }
		public int EventImageOriginalHeight { get; set; }
		public int CacheTime { get; set; }
		public int TimeToDeleteInMinutes { get; set; }
    }
}
