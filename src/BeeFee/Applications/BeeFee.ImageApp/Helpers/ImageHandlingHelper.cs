using System.IO;
using System.Threading.Tasks;
using SharpFuncExt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace BeeFee.ImageApp.Helpers
{
	internal static class ImageHandlingHelper
	{
		internal static async Task SaveImage(Image<Rgba32> image, string path)
			=> await Task.Run(() => image.Save(path, new JpegEncoder {Quality = 85}));

		internal static Image<Rgba32> ResizeImage(Image<Rgba32> image, ImageSize size)
			=> image.Clone().Fluent(z => z.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height) })));

		internal static async Task ResizeAndSave(Image<Rgba32> image, ImageSize size, string path)
			=> await SaveImage(ResizeImage(image, size), path);

		internal static void DeleteImage(string path)
			=> File.Delete(path);
	}
}