using System;
using System.Collections.Generic;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Embed
{
	public class ImageAppStartSettings
	{
		public Size MinimalSize { get; set; }
		public Size MaximalSize { get; set; }
		public TimeSpan TimeForCachingKeys { get; set; }
		public List<string> AdminIps { get; set; }
		public string TempDirectory { get; set; }
	}
}