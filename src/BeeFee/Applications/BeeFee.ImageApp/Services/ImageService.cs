using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp.Exceptions;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Newtonsoft.Json;
using SixLabors.Primitives;
using SharpFuncExt;

namespace BeeFee.ImageApp.Services
{
	public class ImageService
	{
		private readonly string _folder;
		private readonly string _publicOriginalFolder;
		private readonly string _privateOriginalFolder;
		private readonly ImageSize _maxOriginalSize;
		private readonly ConcurrentDictionary<string, ImageSettings> _settings;
		private readonly string _settingsJsonFile;
		private readonly object _locker = new object();

		public ImageService(string folder, string publicOriginalFolder, string privateOriginalFolder, ImageSize maxOriginalSize, string settingsJsonFile)
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

			_settingsJsonFile = settingsJsonFile;
			if (!File.Exists(settingsJsonFile)) File.Create(settingsJsonFile);
			_settings = DeserializeSettings() ?? new ConcurrentDictionary<string, ImageSettings>();
		}

		public Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, string settingName,
			string key)
			=> _settings
				.ThrowIf(x => !CheckKey(eventName, key), x => new AccessDeniedException())
				.GetValueOrDefault(settingName).Fluent(x => Console.WriteLine($"Add file {fileName}, setting: {settingName}"))
				.IfNotNull(x => AddImage(stream, eventName, fileName, x, key),
					Task.FromResult(new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}",
						EErrorType.SettingNotFound)));
		//{
		//	ImageSettings setting;
		//	try
		//	{
		//		setting = _settings[settingName];
		//	}
		//	catch (KeyNotFoundException)
		//	{
		//		return Task.FromResult(new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}", EErrorType.SettingNotFound));
		//	}

		//	return AddImage(stream, fileName, setting);
		//}

		internal Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, ImageSettings setting,
			string key)
			=> GetUniqueName(eventName, fileName)
				.ThrowIf(x => !CheckKey(eventName, key), x => new AccessDeniedException())
				.Try(uniqueName => stream
						//.If(s => FileExists(uniqueName),
						//	n => new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} already exists", EErrorType.FileAlreadyExists))
						.ThrowIf(
							s => FileExists(uniqueName),
							n => new FileAlreadyExistsException($"File \"{uniqueName}\" already exists"))
						.Using(s => Image.Load(s).Using(image => Task.WhenAll(new Task[0]
							.Add(ResizeAndSaveImage(image, _maxOriginalSize,
								setting.KeepPublicOriginalSize ? _publicOriginalFolder : _privateOriginalFolder, uniqueName))
							.AddEach(setting.Sizes.Select(x => ResizeAndSaveImage(image, x, eventName, uniqueName))))
						)),
					x => new ImageOperationResult(EAddImageResut.Ok, x),
					(x, e) => new ImageOperationResult(EAddImageResut.Error, x, e.Message, EErrorType.SaveImageError));
		

		private bool FileExists(string filename)
			//=> File.Exists(Path.Combine(_folder, path, filename));
			=> File.Exists(Path.Combine(_folder, _privateOriginalFolder, filename)) ||
			   File.Exists(Path.Combine(_folder, _publicOriginalFolder, filename));

		//{
		//	var uniqueName = GetUniqueName(fileName);
		//	try
		//	{
		//		using (stream)
		//		{
		//			var image = Image.Load(stream);
		//			await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _privateOriginalFolder, uniqueName));
		//			if (setting.KeepPublicOriginalSize)
		//				await Task.Run(() => SaveImage(Resize(image, _maxOriginalSize), _publicOriginalFolder, uniqueName));

		//			if (setting.Sizes != null) 
		//				foreach (var size in setting.Sizes)
		//					await Task.Run(() => SaveImage(Resize(image, size), size, uniqueName));
		//		}
		//	}
		//	catch (ArgumentNullException e)
		//	{
		//		return new ImageOperationResult(EAddImageResut.Error, uniqueName, e.Message, EErrorType.SaveImageError);
		//	}
		//	return new ImageOperationResult(EAddImageResut.Ok, uniqueName);
		//}

		public ImageOperationResult RemoveImage(string eventName, string fileName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!File.Exists(Path.Combine(_folder, eventName, _privateOriginalFolder, fileName))) 
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			foreach (var directory in Directory.GetDirectories(Path.Combine(_folder, eventName)))
			{
				File.Delete(Path.Combine(directory, fileName));
			}
			return new ImageOperationResult(EAddImageResut.Ok, fileName);
		}

		public ImageOperationResult RenameImage(string eventName, string oldName, string newName, string key, bool canChangeName = true)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if(!File.Exists(Path.Combine(_folder, _privateOriginalFolder, oldName)))
				return new ImageOperationResult(EAddImageResut.Error, oldName, $"File {oldName} doesn't exists", EErrorType.FileDoesNotExists);

			if (!canChangeName && File.Exists(Path.Combine(_folder, _privateOriginalFolder, newName)))
				return new ImageOperationResult(EAddImageResut.Error, newName, $"File {newName} already exists", EErrorType.FileAlreadyExists);

			var uniqueName = GetUniqueName(eventName, newName);

			foreach (var directory in Directory.GetDirectories(_folder))
			{
				if (!File.Exists(Path.Combine(directory, oldName))) continue;
				File.Move(Path.Combine(directory, oldName), Path.Combine(directory, uniqueName));
			}

			return new ImageOperationResult(EAddImageResut.Ok, uniqueName);
		}

		public async Task<ImageOperationResult> UpdateImage(Stream stream, string eventName, string fileName, string settingName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!File.Exists(Path.Combine(_folder, _privateOriginalFolder, fileName)))
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			ImageSettings setting;
			try
			{
				setting = _settings[settingName];
			}
			catch (System.Collections.Generic.KeyNotFoundException)
			{
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}", EErrorType.SettingNotFound);
			}

			var resolutions = new System.Collections.Generic.List<ImageSize>();
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
			
			RemoveImage(eventName, fileName, key);
			return await AddImage(stream, eventName, fileName,
				new ImageSettings(resolutions.ToHashSet().ToArray(), _settings[settingName].KeepPublicOriginalSize), key);
		}

		public void RegisterEvent(string eventName, string key)
		{
			if (Directory.Exists(Path.Combine(_folder, eventName))) throw new DirectoryAlreadyExistsException();

			Directory.CreateDirectory(Path.Combine(_folder, eventName));
			File.Create($"{key}.key");
		}

		private bool CheckKey(string eventName, string key)
			=> File.Exists(Path.Combine(_folder, eventName, $"{key}.key"));

		#region Private Methods

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
			=> new Image<Rgba32>(image).Resize(new ResizeOptions {Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height)});

		private void SaveImage(Image<Rgba32> image, string eventName, string sizePath, string fileName)
		{
			var directory = Path.Combine(_folder, eventName, sizePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var fullPath = Path.Combine(directory, fileName);

			image.Save(fullPath, new JpegEncoder {Quality = 85});
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
		#endregion Private Methods

	}
}