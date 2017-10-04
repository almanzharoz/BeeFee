using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Helpers;
using BeeFee.Model;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharpFuncExt;
using SixLabors.ImageSharp;

namespace BeeFee.ImageApp.Services
{
	public class ImageService
	{
		private readonly MemoryCacheManager _cacheManager;
		private readonly ConcurrentDictionary<string, ImageSettings> _settings;
		private readonly ImageSize _userAvatarSize;
		private readonly ImageSize _companyLogoSize;
		private readonly ImageSize _eventImageOriginalMaxSize;
		private readonly PathHandler _pathHandler;
		private readonly int _cacheTime;
		private readonly object _locker;
		private readonly string _settingsJsonFile;
		private readonly TimeSpan _timeToDelete;

		public ImageService(MemoryCacheManager cacheManager, string settingsJsonFile,
			ImageSize userAvatarSize, ImageSize companyLogoSize, ImageSize eventImageOriginalMaxSize, PathHandler pathHandler,
			int cacheTime, int timeToDeleteInMinutes)
		{
			_cacheManager = cacheManager;
			_locker = new object();

			_settingsJsonFile = settingsJsonFile;
			if (!File.Exists(settingsJsonFile)) File.Create(settingsJsonFile).Dispose();
			_settings = DeserializeSettings() ?? new ConcurrentDictionary<string, ImageSettings>();

			_userAvatarSize = userAvatarSize;
			_companyLogoSize = companyLogoSize;
			_eventImageOriginalMaxSize = eventImageOriginalMaxSize;
			_pathHandler = pathHandler;
			_cacheTime = cacheTime;
			_timeToDelete = TimeSpan.FromMinutes(timeToDeleteInMinutes);
		}

		/// <summary>
		/// Add Company Logo to Image server
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		public async Task AddCompanyLogo(Stream stream, string companyUrl, string key)
			=> await AddLogoOrAvatar(stream, companyUrl, key, EImageType.CompanyLogo);

		/// <summary>
		/// Add User Avatar to Image server
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		public async Task AddUserAvatar(Stream stream, string userName, string key)
			=> await AddLogoOrAvatar(stream, userName, key, EImageType.UserAvatar);

		/// <summary>
		/// Add image for event
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		public async Task<ImageOperationResult> AddEventImage(Stream stream, string companyName, string eventName,
			string fileName, string settingName, string key)
		{
			if (!IsKeyValid(key, companyName)) throw new AccessDeniedException();
			if (!_settings.TryGetValue(settingName, out var setting))
				throw new KeyNotFoundException($"setting with {settingName} not found");

			if (setting.CanChangeName)
				fileName = _pathHandler.GetUniqueName(companyName, eventName, fileName);
			else if (_pathHandler.IsEventImageExists(companyName, eventName, fileName))
				return new ImageOperationResult(EImageOperationResult.Error, fileName, $"File {fileName} already exists",
					EErrorType.FileAlreadyExists);

			using (var image = Image.Load(stream))
			{
				await AddOriginalImage(image, companyName, eventName, fileName,
					setting.KeepPublicOriginalSize ? EImageType.EventPublicOriginalImage : EImageType.EventPrivateOriginalImage);
				await AddResizedImages(image, companyName, eventName, fileName, setting.Sizes);
			}

			return new ImageOperationResult(EImageOperationResult.Ok, fileName);
		}

		/// <summary>
		/// Removes event image
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public void RemoveEventImage(string companyName, string eventName, string fileName, string key)
		{
			if(!IsKeyValid(key, companyName) || 
				_pathHandler.IsFileLivesLessThan(_timeToDelete, _pathHandler.FindPathToOriginalEventImage(companyName, eventName, fileName)))
				throw new AccessDeniedException();
			if(!_pathHandler.IsEventImageExists(companyName, eventName, fileName)) throw new FileNotFoundException();

			_pathHandler.GetAllSizePathToEventImage(companyName, eventName, fileName)
				.Each(ImageHandlingHelper.DeleteImage);
			DeleteOriginalImage(companyName, eventName, fileName);
		}

		/// <summary>
		/// Renames EventImages
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public ImageOperationResult RenameEventImage(string companyName, string eventName, string oldFileName, string newFileName, bool canChangeName, string key)
		{
			if(!IsKeyValid(key, companyName)) throw new AccessDeniedException();
			if (!_pathHandler.IsEventImageExists(companyName, eventName, oldFileName)) throw new FileNotFoundException();

			try
			{
				newFileName = RenameOriginalImage(companyName, eventName, oldFileName, newFileName, canChangeName);
			}
			catch (FileAlreadyExistsException)
			{
				return new ImageOperationResult(EImageOperationResult.Error, newFileName, $"File {newFileName} already exists", EErrorType.FileAlreadyExists);
			}

			_pathHandler.GetAllSizePathToEventImageForRename(companyName, eventName, oldFileName, newFileName)
				.Each(x => ImageHandlingHelper.RenameImage(x.OldPath, x.NewPath));

			return new ImageOperationResult(EImageOperationResult.Ok, newFileName);
		}

		public async Task<ImageOperationResult> UpdateEventImage(Stream stream, string companyName, string eventName,
			string fileName, string settingName, string key)
		{
			if (!IsKeyValid(key, companyName)) throw new AccessDeniedException();
			if (!_settings.TryGetValue(settingName, out var setting))
				throw new KeyNotFoundException($"setting with {settingName} not found");

			var sizes = _pathHandler.GetImageSizes(companyName, eventName, fileName);
			RemoveEventImage(companyName, eventName, fileName, key);

			using (var image = Image.Load(stream))
			{
				await AddOriginalImage(image, companyName, eventName, fileName,
					setting.KeepPublicOriginalSize ? EImageType.EventPublicOriginalImage : EImageType.EventPrivateOriginalImage);
				await AddResizedImages(image, companyName, eventName, fileName, setting.Sizes.ToHashSet().Concat(sizes));
			}

			return new ImageOperationResult(EImageOperationResult.Ok, fileName);
		}

		public void GetAccessToFolder(string key, string directoryName)
		{
			var fullKey = MakeKey(key, directoryName);
			if (_cacheManager.IsSet(fullKey))
				_cacheManager.Remove(fullKey);
			_cacheManager.Set(fullKey, new MemoryCacheKeyObject(EKeyType.User, directoryName), _cacheTime);
		}

		private static string MakeKey(string key, string directoryName)
			=> key + directoryName;

		private async Task AddLogoOrAvatar(Stream stream, string name, string key, EImageType imageType)
		{
			if (!IsKeyValid(key, name)) throw new AccessDeniedException();

			using (var image = Image.Load(stream))
			{
				var path = _pathHandler.GetPathToLogoOrAvatar(name, imageType);
				if (_pathHandler.IsAvatarOrLogoExists(name, imageType))
					ImageHandlingHelper.DeleteImage(path);
				await ImageHandlingHelper.ResizeAndSave(image, GetMaxSizeByImageType(imageType), path);
			}
		}

		private async Task AddOriginalImage(Image<Rgba32> image, string companyName, string eventName, string fileName, EImageType imageType)
		{
			if(imageType != EImageType.EventPrivateOriginalImage && imageType != EImageType.EventPublicOriginalImage)
				throw new ArgumentException("Image type must be EventPrivateOriginalImage or EventPublicOriginalImage");

			await ImageHandlingHelper.ResizeAndSave(image, GetMaxSizeByImageType(imageType),
				_pathHandler.GetPathToOriginalImage(companyName, eventName, imageType, fileName));
		}

		private async Task AddResizedImages(Image<Rgba32> image, string companyName, string eventName, string fileName,
			IEnumerable<ImageSize> sizes)
		{
			foreach (var size in sizes)
				await ImageHandlingHelper.ResizeAndSave(image, size,
					_pathHandler.GetPathToImageSize(companyName, eventName, size, fileName));
		}

		private void DeleteOriginalImage(string companyName, string eventName, string fileName)
			=> ImageHandlingHelper.DeleteImage(_pathHandler.FindPathToOriginalEventImage(companyName, eventName, fileName));

		private string RenameOriginalImage(string companyName, string eventName, string oldFileName, string newFileName,
			bool canChangeName)
		{
			if(!canChangeName && _pathHandler.IsEventImageExists(companyName, eventName, newFileName))
				throw new FileAlreadyExistsException();
			newFileName = _pathHandler.GetUniqueName(companyName, eventName, newFileName);
			ImageHandlingHelper.RenameImage(
				_pathHandler.FindPathToOriginalEventImageForRename(companyName, eventName, oldFileName, newFileName));
			return newFileName;
		}

		private ImageSize GetMaxSizeByImageType(EImageType imageType)
		{
			switch (imageType)
			{
				case EImageType.CompanyLogo:
					return _companyLogoSize;
				case EImageType.UserAvatar:
					return _userAvatarSize;
				case EImageType.EventPrivateOriginalImage:
					return _eventImageOriginalMaxSize;
				case EImageType.EventPublicOriginalImage:
					return _eventImageOriginalMaxSize;
				case EImageType.EventResizedImage:
					return _eventImageOriginalMaxSize;
				default:
					throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null);
			}
		}

		private bool IsKeyValid(string key, string directoryName)
			=> _cacheManager.IsSet(MakeKey(key, directoryName)) &&
			   _cacheManager.Get<MemoryCacheKeyObject>(MakeKey(key, directoryName)).Directory == directoryName;

		public void SetSetting(string settingName, ImageSettings setting, string key)
		{
			if (_cacheManager.IsSet(key) && _cacheManager.Get<MemoryCacheKeyObject>(key).Type == EKeyType.Moderator)
				_settings.AddOrUpdate(settingName, setting,
					(s, settings) => settings.Set(setting.Sizes, setting.KeepPublicOriginalSize, setting.CanChangeName));
			SerializeSettings();
		}

		private void SerializeSettings()
		{
			lock (_locker)
				File.WriteAllText(_settingsJsonFile, JsonConvert.SerializeObject(_settings));
		}

		private ConcurrentDictionary<string, ImageSettings> DeserializeSettings()
		{
			if(!File.Exists(_settingsJsonFile) || string.IsNullOrEmpty(File.ReadAllText(_settingsJsonFile)))
				return null;
			lock (_locker)
				return JsonConvert.DeserializeObject<ConcurrentDictionary<string, ImageSettings>>(File.ReadAllText(_settingsJsonFile));
		}

		public void RegisterEvent(string companyName, string eventName)
		{
			_pathHandler.CreateEventFolder(companyName, eventName);
		}
	}
}