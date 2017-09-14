using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Newtonsoft.Json;
using SixLabors.Primitives;

namespace BeeFee.ImageApp.Services
{
	public class ImageService
	{
		private readonly string _folder;
		private readonly string _publicOriginalFolder;
		private readonly string _privateOriginalFolder;
		private readonly ImageSize _maxOriginalSize;
		private readonly Dictionary<string, ImageSettings> _settings;
		private const string SettingsJsonFile = "settings.json";

		public ImageService(string folder, string publicOriginalFolder, string privateOriginalFolder, ImageSize maxOriginalSize)
		{
			_folder = folder;
			_publicOriginalFolder = publicOriginalFolder;
			_privateOriginalFolder = privateOriginalFolder;
			_maxOriginalSize = maxOriginalSize;

			_settings = DeserializeSettings();
		}

		public async Task<AddImageResult> AddImage(Stream stream, string fileName, string settingName)
		{
			ImageSettings setting;
			try
			{
				setting = _settings[settingName];
			}
			catch (KeyNotFoundException)
			{
				return new AddImageResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}");
			}

			var uniqueName = GetUniqueName(fileName);
			try
			{
				using (stream)
				{
					var image = Image.Load(stream);
					await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _privateOriginalFolder, uniqueName));
					if (setting.KeepPublicOriginalSize)
						await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _publicOriginalFolder, uniqueName));

					if (setting.Sizes != null) 
						foreach (var size in setting.Sizes)
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

		public void SetSetting(string name, ImageSize[] sizes, bool keepPublicOriginalSize)
		{
			_settings.Add(name, new ImageSettings(sizes, keepPublicOriginalSize));
			SerializeSettings();
		}

		private void SerializeSettings()
		{
			var text = JsonConvert.SerializeObject(_settings);
			File.WriteAllText(SettingsJsonFile, text);
		}

		private static Dictionary<string, ImageSettings> DeserializeSettings()
		{
			var file = File.ReadAllText(SettingsJsonFile);
			return JsonConvert.DeserializeObject<Dictionary<string, ImageSettings>>(file);
		}

	}
}