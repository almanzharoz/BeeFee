﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Helpers;
using SharpFuncExt;
using SixLabors.ImageSharp;

namespace BeeFee.ImageApp.Services
{
	public partial class ImageService
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
		public Task<ImageOperationResult> AddCompanyLogo(Stream stream, string companyUrl, string key)
			=> AddLogoOrAvatar(stream, companyUrl, key, EImageType.CompanyLogo);

		/// <summary>
		/// Add User Avatar to Image server
		/// </summary>
		/// <exception cref="AccessDeniedException"></exception>
		public Task<ImageOperationResult> AddUserAvatar(Stream stream, string userName, string key)
			=> AddLogoOrAvatar(stream, userName, key, EImageType.UserAvatar);

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
				var t = AddOriginalImage(image, companyName, eventName, fileName,
					setting.KeepPublicOriginalSize ? EImageType.EventPublicOriginalImage : EImageType.EventPrivateOriginalImage);
				await AddResizedImages(image, companyName, eventName, fileName, setting.Sizes);
				await t; // Пока сохраняется большая, успеет сохраниться и часть маленьких
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
			//if (_cacheManager.IsSet(fullKey))
				//_cacheManager.Remove(fullKey); // TODO: А зачем здесь Remove и дальнейший Set?
			if (!_cacheManager.IsSet(fullKey))
				_cacheManager.Set(fullKey, new MemoryCacheKeyObject(EKeyType.User, directoryName), _cacheTime);
		}

		public bool RegisterEvent(string companyName, string eventName, string key)
		{
			try
			{
				if (IsKeyValid(key, companyName))
				{
					_pathHandler.CreateEventFolder(companyName, eventName);
					return true;
				}
			}
			catch
			{
				//TODO: Логирование
			}
			return false;
		}

		public void SetSetting(string settingName, ImageSettings setting, string key)
		{
			if (_cacheManager.IsSet(key) && _cacheManager.Get<MemoryCacheKeyObject>(key).Type == EKeyType.Moderator)
				_settings.AddOrUpdate(settingName, setting,
					(s, settings) => settings.Set(setting.Sizes, setting.KeepPublicOriginalSize, setting.CanChangeName));
			SerializeSettings();
		}

	}
}