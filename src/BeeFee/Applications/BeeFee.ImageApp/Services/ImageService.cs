using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Newtonsoft.Json;
using SixLabors.Primitives;
using SharpFuncExt;

namespace BeeFee.ImageApp.Services
{
	public partial class ImageService
	{
		private readonly string _folder;
		private readonly string _publicOriginalFolder;
		private readonly string _privateOriginalFolder;
		private readonly string _resizedFolder;
		private readonly ImageSize _maxOriginalSize;
		private readonly ConcurrentDictionary<string, ImageSettings> _settings;
		private readonly string _settingsJsonFile;
		private readonly object _locker = new object();
		private readonly TimeSpan _removeImageAvailabilityTime;

		public ImageService(string folder, string publicOriginalFolder, string privateOriginalFolder, string resizedFolder,
			ImageSize maxOriginalSize, string settingsJsonFile, TimeSpan removeImageAvailabilityTime)
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

			_resizedFolder = resizedFolder;
			if (!Directory.Exists(resizedFolder))
				Directory.CreateDirectory(Path.Combine(folder, resizedFolder));

			_maxOriginalSize = maxOriginalSize;

			_settingsJsonFile = settingsJsonFile;
			if (!File.Exists(settingsJsonFile)) File.Create(settingsJsonFile).Dispose();
			_settings = DeserializeSettings() ?? new ConcurrentDictionary<string, ImageSettings>();

			_removeImageAvailabilityTime = removeImageAvailabilityTime;
		}

		public Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, string settingName, string key)
			=> EnumerableFunc.GetValueOrDefault(_settings, settingName).Fluent(x => Console.WriteLine($"Add file {fileName}, setting: {settingName}"))
				.IfNotNull(x => AddImage(stream, eventName.HasNotNullArg(nameof(eventName)), fileName.HasNotNullArg(nameof(fileName)), x, key.HasNotNullArg(nameof(key))),
					Task.FromResult(new ImageOperationResult(EImageOperationResult.Error, fileName, $"Cannot found a setting {settingName}",
						EErrorType.SettingNotFound)));
		

		internal Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, ImageSettings setting, string key)
			=> GetUniqueName(eventName, fileName)
				.ThrowIf(x => !CheckKey(eventName, key), x => new AccessDeniedException())
				.Try(uniqueName => stream
						//.If(s => FileExists(uniqueName),
						//	n => new ImageOperationResult(EImageOperationResult.Error, fileName, $"File {fileName} already exists", EErrorType.FileAlreadyExists))
						.ThrowIf(
							s => FileExists(uniqueName, eventName),
							n => new FileAlreadyExistsException($"File \"{uniqueName}\" already exists"))
						.Using(s => Image.Load(s).Using(image => Task.WhenAll(new Task[0]
							.Add(ResizeAndSaveImage(image, _maxOriginalSize, eventName,
								setting.KeepPublicOriginalSize, uniqueName))
							.AddEach(setting.Sizes.Select(x => ResizeAndSaveImage(image, x, eventName, uniqueName))))
						)),
					x => new ImageOperationResult(EImageOperationResult.Ok, x),
					(x, e) => new ImageOperationResult(EImageOperationResult.Error, x, e.Message, EErrorType.SaveImageError));

		public ImageOperationResult RemoveImage(string eventName, string fileName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!FileExists(fileName, eventName)) 
				return new ImageOperationResult(EImageOperationResult.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			var privateFile = Path.Combine(GetPathToPrivateOriginalFolder(eventName), fileName);
			var publicFile = Path.Combine(GetPathToPublicOriginalFolder(eventName), fileName);
			if (File.Exists(privateFile) && File.GetCreationTimeUtc(privateFile).Add(_removeImageAvailabilityTime) < DateTime.UtcNow ||
				File.Exists(publicFile) && File.GetCreationTimeUtc(publicFile).Add(_removeImageAvailabilityTime) < DateTime.UtcNow)
				throw new AccessDeniedException("Time for remove the file is over");

			if(File.Exists(privateFile))
				File.Delete(privateFile);
			if(File.Exists(publicFile))
				File.Delete(publicFile);

			foreach (var directory in Directory.GetDirectories(GetPathToResizedFolder(eventName)))
			{
				File.Delete(Path.Combine(directory, fileName));
			}
			return new ImageOperationResult(EImageOperationResult.Ok, fileName);
		}

		public ImageOperationResult RenameImage(string eventName, string oldName, string newName, string key, bool canChangeName = true)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if(!FileExists(oldName, eventName))
				return new ImageOperationResult(EImageOperationResult.Error, oldName, $"File {oldName} doesn't exists", EErrorType.FileDoesNotExists);

			if (!canChangeName && FileExists(oldName, eventName))
				return new ImageOperationResult(EImageOperationResult.Error, newName, $"File {newName} already exists", EErrorType.FileAlreadyExists);

			var uniqueName = GetUniqueName(eventName, newName);

			var resized = GetPathToResizedFolder(eventName);
			var directories = Directory.GetDirectories(GetPathToResizedFolder(eventName));
			foreach (var directory in Directory.GetDirectories(GetPathToResizedFolder(eventName)))
			{
				if (!File.Exists(Path.Combine(directory, oldName))) continue;
				File.Move(Path.Combine(directory, oldName), Path.Combine(directory, uniqueName));
			}

			if (File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), oldName)))
				File.Move(Path.Combine(GetPathToPrivateOriginalFolder(eventName), oldName),
					Path.Combine(GetPathToPrivateOriginalFolder(eventName), uniqueName));

			if (File.Exists(Path.Combine(GetPathToPublicOriginalFolder(eventName), oldName)))
				File.Move(Path.Combine(GetPathToPublicOriginalFolder(eventName), oldName),
					Path.Combine(GetPathToPrivateOriginalFolder(eventName), uniqueName));

			return new ImageOperationResult(EImageOperationResult.Ok, uniqueName);
		}

		public async Task<ImageOperationResult> UpdateImage(Stream stream, string eventName, string fileName, string settingName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), fileName)))
				return new ImageOperationResult(EImageOperationResult.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			ImageSettings setting;
			try
			{
				setting = _settings[settingName];
			}
			catch (System.Collections.Generic.KeyNotFoundException)
			{
				return new ImageOperationResult(EImageOperationResult.Error, fileName, $"Cannot found a setting {settingName}", EErrorType.SettingNotFound);
			}

			var resolutions = new List<ImageSize>();
			foreach (var directory in Directory.GetDirectories(GetPathToResizedFolder(eventName)))
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

		public string RegisterEvent(string eventName)
		{
			if (Directory.Exists(GetPathToPrivateOriginalFolder(eventName))) throw new DirectoryAlreadyExistsException();

			var key = Guid.NewGuid().ToString();
			//Directory.CreateDirectory(Path.Combine(_folder, eventName));
			CreateEventDirectories(eventName);
			//File.Create($"{key}.key");  // TODO: Добавить хэш
			MakeKeyFile(eventName, key);
			return key;
		}

		public void RenameEvent(string oldName, string newName)
		{
			Directory.Move(GetPathToPrivateOriginalFolder(oldName), GetPathToPrivateOriginalFolder(newName));
			Directory.Move(GetPathToPublicOriginalFolder(oldName), GetPathToPublicOriginalFolder(newName));
			Directory.Move(GetPathToResizedFolder(oldName), GetPathToResizedFolder(newName));
		}

		public void SetSetting(string name, ImageSize[] sizes, bool keepPublicOriginalSize)
		{
			_settings.AddOrUpdate(name, x => new ImageSettings(sizes, keepPublicOriginalSize), (x, y) => y.Set(sizes, keepPublicOriginalSize));
			SerializeSettings();
		}
	}
}