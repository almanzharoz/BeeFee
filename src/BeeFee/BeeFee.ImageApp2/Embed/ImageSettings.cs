using System.Collections.Generic;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Embed
{
	public struct ImageSettings
	{
		public string TempPath { get; }
		public string NewPath { get; }
		public List<Size> Sizes { get; }

		public ImageSettings(string tempPath, string newPath, List<Size> sizes)
		{
			TempPath = tempPath;
			NewPath = newPath;
			Sizes = sizes;
		}
	}
}