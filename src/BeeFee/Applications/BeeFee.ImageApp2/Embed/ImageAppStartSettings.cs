using System;
using System.Collections.Generic;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Embed
{
	public class ImageAppStartSettings
	{
		public Size MinimalSize { get; set; }
		public Size MaximalSize { get; set; }
		public int TimeForCachingKeys { get; set; }
		public List<string> AdminHosts { get; set; }
		public string TempDirectory { get; set; }
	}
}