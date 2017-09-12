using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using SixLabors.Primitives;

namespace BeeFee.ImageApp
{
	public class ImageService
	{
		private readonly string _folder;
		private readonly string _publicOriginalFolder;
		private readonly string _privateOriginalFolder;
		private readonly ImageSize _maxOriginalSize;

		public ImageService(string folder, string publicOriginalFolder, string privateOriginalFolder, ImageSize maxOriginalSize)
		{
			_folder = folder;
			_publicOriginalFolder = publicOriginalFolder;
			_privateOriginalFolder = privateOriginalFolder;
			_maxOriginalSize = maxOriginalSize;
		}

		public async Task<AddImageResult> AddImage(Stream stream, string fileName, ImageSize[] sizes, bool makeOriginalPublic = false)
		{
			var uniqueName = GetUniqueName(fileName);
			try
			{
				using (stream)
				{
					var image = Image.Load(stream);
					await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _privateOriginalFolder, uniqueName));
					if (makeOriginalPublic)
						await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _publicOriginalFolder, uniqueName));

					if (sizes != null)
						foreach (var size in sizes)
							await Task.Run(() => SaveImage(Resize(image, size), size, uniqueName));
				}
			}
			catch (ArgumentException e)
			{
				return new AddImageResult(EAddImageResut.Error, uniqueName, e.Message);
			}
			return new AddImageResult(EAddImageResut.Ok, uniqueName);
		}

		private string GetUniqueName(string fileName)
		{
			var result = fileName;
			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			var fullName = Path.Combine(_folder, _privateOriginalFolder, fileName);
			var index = 0;

			while (File.Exists(fullName))
			{
				result = nameWithoutExtension + index + Path.GetExtension(fileName);
				fullName = Path.Combine(_folder, _privateOriginalFolder, result);
				index++;
			}
			return result;
		}

		private Image<Rgba32> Resize(Image<Rgba32> image, ImageSize size)
		{
			var result = new Image<Rgba32>(image);
			return result.Resize(new ResizeOptions {Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height)});
		}

		private void SaveImage(Image<Rgba32> image, string sizePath, string fileName)
		{
			var directory = Path.Combine(_folder, sizePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var fullPath = Path.Combine(directory, fileName);

			image.Save(fullPath, new JpegEncoder {Quality = 85});
		}

		private void SaveImage(Image<Rgba32> image, ImageSize size, string fileName)
			=> SaveImage(image, size.ToString(), fileName);
	}
}