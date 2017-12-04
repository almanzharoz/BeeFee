using System.Linq;
using BeeFee.ImageApp2.Embed;

namespace BeeFee.ImagesWebApplication2.Models
{
	public class AcceptModel
	{
		public string TempPath { get; set; }
		public ImageSaveSetting[] Images { get; set; }

		public ImageSettings CreateImageSettings()
			=> new ImageSettings(TempPath,
				Images.Select(x =>
					new ImageApp2.Embed.ImageSaveSetting(new SixLabors.Primitives.Size(x.Size.Width, x.Size.Height), x.Path)));
	}

	public struct ImageSaveSetting
	{
		public Size Size { get; set; }
		public string Path { get; set; }
	}

	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }
	}
}