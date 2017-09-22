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
			=> File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), $".{GetMd5(key)}"));

		private void MakeKeyFile(string eventName, string key)
			=> File.Create(Path.Combine(GetPathToPrivateOriginalFolder(eventName), $".{GetMd5(key)}")).Dispose();

		private static string GetMd5(string key)
		{
			var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

			var result = new StringBuilder(md5.Length * 2);

			foreach (var t in md5)
				result.Append(t.ToString("x2"));

			return result.ToString();
		}

		private bool FileExists(string filename, string eventName)
			=> File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), filename)) ||
			   File.Exists(Path.Combine(GetPathToPublicOriginalFolder(eventName), filename));

		private string GetUniqueName(string eventName, string fileName)
		{
			var result = fileName;
			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

			var index = 0;
			var newName = fileName;

			while (FileExists(newName, eventName))
			{
				newName = nameWithoutExtension + index + Path.GetExtension(fileName);
				//fullName = Path.Combine(_folder, _privateOriginalFolder, result);
				index++;
			}
			return newName;
		}

		private Image<Rgba32> Resize(Image<Rgba32> image, ImageSize size)
			=> new Image<Rgba32>(image).Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height) });

		//TODO: переделать сборку папки
		private void SaveImage(Image<Rgba32> image, string sizePath, string fileName)
		{
			var directory = Path.Combine(sizePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var fullPath = Path.Combine(directory, fileName);

			image.Save(fullPath, new JpegEncoder { Quality = 85 });
		}

		private Task ResizeAndSaveImage(Image<Rgba32> image, ImageSize size, string eventName, string fileName)
			=> Resize(image, size).Using(img => Task.Run(() =>
				SaveImage(img, Path.Combine(GetPathToResizedFolder(eventName), size.ToString()), fileName)));

		private Task ResizeAndSaveImage(Image<Rgba32> image, ImageSize size, string eventName, bool keepPublicOriginal,
			string fileName)
			=> Resize(image, size)
				.Fluent(x => Console.WriteLine($"Resized file {fileName}, size: {size}"))
				.Using(img => Task.Run(() => SaveImage(img,
					keepPublicOriginal ? GetPathToPublicOriginalFolder(eventName) : GetPathToPrivateOriginalFolder(eventName),
					fileName)));

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

		private string GetPathToResizedFolder(string eventName)
			=> Path.Combine(_folder, _resizedFolder, eventName);

		private string GetPathToPrivateOriginalFolder(string eventName)
			=> Path.Combine(_folder, _privateOriginalFolder, eventName);

		private string GetPathToPublicOriginalFolder(string eventName)
			=> Path.Combine(_folder, _publicOriginalFolder, eventName);

		private string GetTargetPath(string systemPath, string eventName, string sizePath = null)
			=> sizePath == null ? Path.Combine(systemPath, eventName) : Path.Combine(systemPath, sizePath, eventName);

		private void CreateEventDirectories(string eventName)
		{
			Directory.CreateDirectory(GetPathToPrivateOriginalFolder(eventName));
			Directory.CreateDirectory(GetPathToPublicOriginalFolder(eventName));
			Directory.CreateDirectory(GetPathToResizedFolder(eventName));
		}
	}
}