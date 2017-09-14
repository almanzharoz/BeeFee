using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			_publicOriginalFolder = publicOriginalFolder;
			if (!Directory.Exists(publicOriginalFolder))
				Directory.CreateDirectory(Path.Combine(folder, publicOriginalFolder));

			_privateOriginalFolder = privateOriginalFolder;
			if (!Directory.Exists(privateOriginalFolder))
				Directory.CreateDirectory(Path.Combine(folder, privateOriginalFolder));

			_maxOriginalSize = maxOriginalSize;

			_settings = DeserializeSettings();
		}

		public async Task<ImageOperationResult> AddImage(Stream stream, string fileName, string settingName)
		{
			ImageSettings setting;
			try
			{
				setting = _settings[settingName];
			}
			catch (KeyNotFoundException)
			{
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}", EErrorType.SettingNotFound);
			}

			return await AddImage(stream, fileName, setting);
		}

		internal async Task<ImageOperationResult> AddImage(Stream stream, string fileName, ImageSettings setting)
		{
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
			catch (ArgumentNullException e)
			{
				return new ImageOperationResult(EAddImageResut.Error, uniqueName, e.Message, EErrorType.SaveImageError);
			}
			return new ImageOperationResult(EAddImageResut.Ok, uniqueName);
		}

		public ImageOperationResult RemoveImage(string fileName)
		{
			if (!File.Exists(Path.Combine(_folder, _privateOriginalFolder, fileName))) 
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			foreach (var directory in Directory.GetDirectories(_folder))
			{
				File.Delete(Path.Combine(directory, fileName));
			}
			return new ImageOperationResult(EAddImageResut.Ok, fileName);
		}

		public ImageOperationResult RenameImage(string oldName, string newName, bool canChangeName = true)
		{
			if(!File.Exists(Path.Combine(_folder, _privateOriginalFolder, oldName)))
				return new ImageOperationResult(EAddImageResut.Error, oldName, $"File {oldName} doesn't exists", EErrorType.FileDoesNotExists);

			if (!canChangeName && File.Exists(Path.Combine(_folder, _privateOriginalFolder, newName)))
				return new ImageOperationResult(EAddImageResut.Error, newName, $"File {newName} already exists", EErrorType.FileAlreadyExists);

			var uniqueName = GetUniqueName(newName);

			foreach (var directory in Directory.GetDirectories(_folder))
			{
				File.Move(Path.Combine(directory, oldName), Path.Combine(directory, uniqueName));
			}

			return new ImageOperationResult(EAddImageResut.Ok, uniqueName);
		}

		public async Task<ImageOperationResult> UpdateImage(Stream stream, string fileName, string settingName)
		{
			if (!File.Exists(Path.Combine(_folder, _privateOriginalFolder, fileName)))
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			ImageSettings setting;
			try
			{
				setting = _settings[settingName];
			}
			catch (KeyNotFoundException)
			{
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}", EErrorType.SettingNotFound);
			}

			var resolutions = new List<ImageSize>();
			foreach (var directory in Directory.GetDirectories(_folder))
			{
				if(File.Exists(Path.Combine(directory, fileName)))
					try
					{
						resolutions.Add(new ImageSize(Path.GetDirectoryName(directory)));
					}
					catch (Exception)
					{
						// ignored
					}
			}

			resolutions.AddRange(setting.Sizes);
			
			RemoveImage(fileName);
			return await AddImage(stream, fileName,
				new ImageSettings(resolutions.ToHashSet().ToArray(), _settings[settingName].KeepPublicOriginalSize));
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