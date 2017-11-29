using System.Collections.Generic;
using System.Linq;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Embed
{
	public struct ImageSettings
	{
		public string TempPath { get; }
		//public string NewPath { get; }
		//public Size Size { get; }
		public IReadOnlyList<ImageSaveSetting> ImageSaveSettings{ get; }

		public ImageSettings(string tempPath, params ImageSaveSetting[] imageSaveSettings)
		{
			TempPath = tempPath;
			ImageSaveSettings = imageSaveSettings;
		}

		public ImageSettings(string tempPath, IReadOnlyList<ImageSaveSetting> imageSaveSettings)
		{
			TempPath = tempPath;
			ImageSaveSettings = imageSaveSettings;
		}

		public ImageSettings(string tempPath, IEnumerable<ImageSaveSetting> imageSaveSettings)
		{
			TempPath = tempPath;
			ImageSaveSettings = imageSaveSettings.ToList();
		}
	}

	public struct ImageSaveSetting
	{
		public Size Size;
		public string Path;

		public ImageSaveSetting(Size size, string path)
		{
			Size = size;
			Path = path;
		}
	}
}