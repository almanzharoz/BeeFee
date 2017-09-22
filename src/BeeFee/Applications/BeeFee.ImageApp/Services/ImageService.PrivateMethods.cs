using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Newtonsoft.Json;
using SharpFuncExt;
using SixLabors.Primitives;

namespace BeeFee.ImageApp.Services
{
	public partial class ImageService
	{
		private bool CheckKey(string eventName, string key)
			=> File.Exists(Path.Combine(_folder, eventName, $".{GetMd5(key)}"));

		private void MakeKeyFile(string eventName, string key)
			=> File.Create(Path.Combine(GetPathToPrivateOriginalFolder(eventName), $".{GetMd5(key)}"));

		private string GetMd5(string key)
			=> MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key)).ToString();

		private bool FileExists(string filename, string eventName)
			=> File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), filename)) ||
			   File.Exists(Path.Combine(GetPathToPublicOriginalFolder(eventName), filename));

		private string GetUniqueName(string eventName, string fileName)
		{
			var result = fileName;
			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			var fullName = Path.Combine(_folder, eventName, _privateOriginalFolder, fileName);
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
			=> new Image<Rgba32>(image).Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height) });

		private void SaveImage(Image<Rgba32> image, string eventName, string sizePath, string fileName)
		{
			var directory = Path.Combine(_folder, eventName, sizePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var fullPath = Path.Combine(directory, fileName);

			image.Save(fullPath, new JpegEncoder { Quality = 85 });
		}

		private Task ResizeAndSaveImage(Image<Rgba32> image, ImageSize size, string eventName, string fileName)
			=> Resize(image, size).Using(img => Task.Run(() => SaveImage(img, eventName, size.ToString(), fileName)));

		private Task ResizeAndSaveImage(Image<Rgba32> image, ImageSize size, string eventName, string originalImageFolder, string fileName)
			=> Resize(image, size)
				.Fluent(x => Console.WriteLine($"Resized file {fileName}, size: {size}"))
			.Using(img => Task.Run(() => SaveImage(img, eventName, originalImageFolder, fileName)));

		public void SetSetting(string name, ImageSize[] sizes, bool keepPublicOriginalSize)
		{
			_settings.AddOrUpdate(name, x => new ImageSettings(sizes, keepPublicOriginalSize), (x, y) => y.Set(sizes, keepPublicOriginalSize));
			SerializeSettings();
		}

		private void SerializeSettings()
		{
			lock (_locker)
				File.WriteAllText(_settingsJsonFile, JsonConvert.SerializeObject(_settings));
		}

		private ConcurrentDictionary<string, ImageSettings> DeserializeSettings()
		{
			lock (_locker)
				return JsonConvert.DeserializeObject<ConcurrentDictionary<string, ImageSettings>>(File.ReadAllText(_settingsJsonFile));
		}

		private string GetPathToResolution(string eventName, ImageSize size)
			=> Path.Combine(_folder, _resizedFolder, eventName, size.ToString());

		private string GetPathToResizedFolder(string eventName)
			=> Path.Combine(_folder, _resizedFolder, eventName);

		private string GetPathToPrivateOriginalFolder(string eventName)
			=> Path.Combine(_folder, _privateOriginalFolder, eventName);

		private string GetPathToPublicOriginalFolder(string eventName)
			=> Path.Combine(_folder, _publicOriginalFolder, eventName);

		private void CreateEventDirectories(string eventName)
		{
			Directory.CreateDirectory(GetPathToPrivateOriginalFolder(eventName));
			Directory.CreateDirectory(GetPathToPublicOriginalFolder(eventName));
			Directory.CreateDirectory(GetPathToResizedFolder(eventName));
		}
	}
}