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
	// TODO: использовать пути publicOrig/event, privateOrig/event, resized/event
	// TODO: Добавить RenameEvent (менять название папки по url мероприятия)
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
		private readonly TimeSpan RemoveImageAvailabilityTime;

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
			if (!File.Exists(settingsJsonFile)) File.Create(settingsJsonFile);
			_settings = DeserializeSettings() ?? new ConcurrentDictionary<string, ImageSettings>();

			RemoveImageAvailabilityTime = removeImageAvailabilityTime;
		}

		public Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, string settingName, string key)
			=> _settings
				.GetValueOrDefault(settingName).Fluent(x => Console.WriteLine($"Add file {fileName}, setting: {settingName}"))
				.IfNotNull(x => AddImage(stream, eventName.HasNotNullArg(nameof(eventName)), fileName.HasNotNullArg(nameof(fileName)), x, key.HasNotNullArg(nameof(key))),
					Task.FromResult(new ImageOperationResult(EAddImageResut.Error, fileName, $"Cannot found a setting {settingName}",
						EErrorType.SettingNotFound)));
		

		internal Task<ImageOperationResult> AddImage(Stream stream, string eventName, string fileName, ImageSettings setting, string key)
			=> GetUniqueName(eventName, fileName)
				.ThrowIf(x => !CheckKey(eventName, key), x => new AccessDeniedException())
				.Try(uniqueName => stream
						//.If(s => FileExists(uniqueName),
						//	n => new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} already exists", EErrorType.FileAlreadyExists))
						.ThrowIf(
							s => FileExists(uniqueName, eventName),
							n => new FileAlreadyExistsException($"File \"{uniqueName}\" already exists"))
						.Using(s => Image.Load(s).Using(image => Task.WhenAll(new Task[0]
							.Add(ResizeAndSaveImage(image, _maxOriginalSize,
								setting.KeepPublicOriginalSize ? _publicOriginalFolder : _privateOriginalFolder, uniqueName))
							.AddEach(setting.Sizes.Select(x => ResizeAndSaveImage(image, x, eventName, uniqueName))))
						)),
					x => new ImageOperationResult(EAddImageResut.Ok, x),
					(x, e) => new ImageOperationResult(EAddImageResut.Error, x, e.Message, EErrorType.SaveImageError));

		// TODO: Проверять дату создания картинки
		public ImageOperationResult RemoveImage(string eventName, string fileName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!FileExists(fileName, eventName)) 
				return new ImageOperationResult(EAddImageResut.Error, fileName, $"File {fileName} doesn't exists", EErrorType.FileDoesNotExists);

			var privateFile = Path.Combine(GetPathToPrivateOriginalFolder(eventName), fileName);
			var publicFile = Path.Combine(GetPathToPublicOriginalFolder(eventName), fileName);
			if (File.Exists(privateFile) && File.GetCreationTimeUtc(privateFile).Add(RemoveImageAvailabilityTime) < DateTime.UtcNow ||
				File.Exists(publicFile) && File.GetCreationTimeUtc(publicFile).Add(RemoveImageAvailabilityTime) < DateTime.UtcNow)
				throw new AccessDeniedException("Time for remove file is over");

			foreach (var directory in Directory.GetDirectories(GetPathToResizedFolder(eventName)))
			{
				File.Delete(Path.Combine(directory, fileName));
			}
			return new ImageOperationResult(EAddImageResut.Ok, fileName);
		}

		public ImageOperationResult RenameImage(string eventName, string oldName, string newName, string key, bool canChangeName = true)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if(!File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), oldName)))
				return new ImageOperationResult(EAddImageResut.Error, oldName, $"File {oldName} doesn't exists", EErrorType.FileDoesNotExists);

			if (!canChangeName && File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), newName)))
				return new ImageOperationResult(EAddImageResut.Error, newName, $"File {newName} already exists", EErrorType.FileAlreadyExists);

			var uniqueName = GetUniqueName(eventName, newName);

			foreach (var directory in Directory.GetDirectories(GetPathToResizedFolder(eventName)))
			{
				if (!File.Exists(Path.Combine(directory, oldName))) continue;
				File.Move(Path.Combine(directory, oldName), Path.Combine(directory, uniqueName));
			}

			return new ImageOperationResult(EAddImageResut.Ok, uniqueName);
		}

		public async Task<ImageOperationResult> UpdateImage(Stream stream, string eventName, string fileName, string settingName, string key)
		{
			if(!CheckKey(eventName, key)) throw new AccessDeniedException();

			if (!File.Exists(Path.Combine(GetPathToPrivateOriginalFolder(eventName), fileName)))
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
	}
}