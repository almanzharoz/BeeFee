using System.Collections.Generic;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Embed
{
	public struct ImageSettings
	{
		public string TempPath { get; }
		public string NewPath { get; }
		public Size Size { get; }

		public ImageSettings(string tempPath, string newPath, Size size)
		{
			TempPath = tempPath;
			NewPath = newPath;
			Size = size;
		}
	}
}