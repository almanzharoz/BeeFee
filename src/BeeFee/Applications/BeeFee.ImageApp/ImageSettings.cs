namespace BeeFee.ImageApp
{
	internal class ImageSettings
	{
		public ImageSize[] Sizes { get; }
		public bool KeepPublicOriginalSize { get; }

		public ImageSettings(ImageSize[] sizes, bool keepPublicOriginalSize)
		{
			Sizes = sizes;
			KeepPublicOriginalSize = keepPublicOriginalSize;
		}
	}
}