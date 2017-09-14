using Newtonsoft.Json;

namespace BeeFee.ImageApp
{
	internal class ImageSettings
	{
		[JsonProperty]
		public ImageSize[] Sizes { get; private set; }
		[JsonProperty]
		public bool KeepPublicOriginalSize { get; private set; }

		public ImageSettings() { } // For Deserialize
		public ImageSettings(ImageSize[] sizes, bool keepPublicOriginalSize)
			=> Set(sizes, keepPublicOriginalSize);

		public ImageSettings Set(ImageSize[] sizes, bool keepPublicOriginalSize)
		{
			Sizes = sizes;
			KeepPublicOriginalSize = keepPublicOriginalSize;
			return this;
		}
	}
}