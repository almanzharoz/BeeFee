using Newtonsoft.Json;

namespace BeeFee.ImageApp
{
	public class ImageSettings
	{
		[JsonProperty]
		public ImageSize[] Sizes { get; private set; }
		[JsonProperty]
		public bool KeepPublicOriginalSize { get; private set; }
		[JsonProperty]
		public bool CanChangeName { get; private set; }

		public ImageSettings() { } // For Deserialize
		public ImageSettings(ImageSize[] sizes, bool keepPublicOriginalSize, bool canChangeName)
			=> Set(sizes, keepPublicOriginalSize, canChangeName);

		public ImageSettings Set(ImageSize[] sizes, bool keepPublicOriginalSize, bool canChangeName)
		{
			Sizes = sizes;
			KeepPublicOriginalSize = keepPublicOriginalSize;
			CanChangeName = canChangeName;
			return this;
		}
	}
}