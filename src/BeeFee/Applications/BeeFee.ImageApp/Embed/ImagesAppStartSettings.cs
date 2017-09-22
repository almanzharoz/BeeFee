using System;

namespace BeeFee.ImageApp
{
    public class ImagesAppStartSettings
    {
		public string ImagesFolder { get; set; }
		public string PublicOriginalFolder { get; set; }
		public string PrivateOriginalFolder { get; set; }
		public string ResizedFolder { get; set; }
		public int MaxOriginalWidth { get; set; }
		public int MaxOriginalHeight { get; set; }
		public TimeSpan RemoveImageAvailabilityTime { get; set; }
    }
}
