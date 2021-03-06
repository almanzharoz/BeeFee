﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Helpers;
using SixLabors.ImageSharp;
using Newtonsoft.Json;
using SharpFuncExt;

namespace BeeFee.ImageApp.Services
{
	public partial class ImageService
	{
		private static string MakeKey(string key, string directoryName)
			=> key + directoryName;

		private async Task<ImageOperationResult> AddLogoOrAvatar(Stream stream, string name, string key, EImageType imageType)
		{
			if (!IsKeyValid(key, name)) throw new AccessDeniedException();

			using (var image = Image.Load(stream))
			{
				var path = _pathHandler.GetPathToLogoOrAvatar(name, imageType);
				if (_pathHandler.IsAvatarOrLogoExists(name, imageType))
					ImageHandlingHelper.DeleteImage(path);
				await ImageHandlingHelper.ResizeAndSave(image, GetMaxSizeByImageType(imageType), path);
				return new ImageOperationResult(EImageOperationResult.Ok, path); // TODO: Hack
			}
		}

		private Task AddOriginalImage(Image<Rgba32> image, string companyName, string eventName, string fileName,
			EImageType imageType)
		{
			if (imageType != EImageType.EventPrivateOriginalImage && imageType != EImageType.EventPublicOriginalImage)
				throw new ArgumentException("Image type must be EventPrivateOriginalImage or EventPublicOriginalImage");

			return ImageHandlingHelper.ResizeAndSave(image, GetMaxSizeByImageType(imageType),
				_pathHandler.GetPathToOriginalImage(companyName, eventName, imageType, fileName));
		}

		private Task AddResizedImages(Image<Rgba32> image, string companyName, string eventName, string fileName,
			IEnumerable<ImageSize> sizes)
			=> Task.WhenAll(sizes.Select(size => ImageHandlingHelper.ResizeAndSave(image, size,
				_pathHandler.GetPathToImageSize(companyName, eventName, size, fileName))));

		private void DeleteOriginalImage(string companyName, string eventName, string fileName)
			=> ImageHandlingHelper.DeleteImage(_pathHandler.FindPathToOriginalEventImage(companyName, eventName, fileName));

		private string RenameOriginalImage(string companyName, string eventName, string oldFileName, string newFileName,
			bool canChangeName)
		{
			if (!canChangeName && _pathHandler.IsEventImageExists(companyName, eventName, newFileName))
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

		private bool IsKeyValid(string key, string companyName, string eventName)
			=> _cacheManager.Get<MemoryCacheKeyObject>(MakeKey(key, Path.Combine(companyName, eventName)))
					.Convert(x => x != null && x.Directory == Path.Combine(companyName, eventName)) ||
				_cacheManager.Get<MemoryCacheKeyObject>(MakeKey(key, companyName))
					.Convert(x => x != null && x.Directory == companyName && x.HasAccessToSubdirectories) ||
				_cacheManager.Get<MemoryCacheKeyObject>(key) // TODO: Для сервера проверять не через кэш
					.Convert(x => x != null && x.IsAdminKey);

		private bool IsKeyValid(string key, string companyName)
			=> _cacheManager.Get<MemoryCacheKeyObject>(MakeKey(key, companyName))
					.Convert(x => x != null && x.Directory == companyName) ||
				_cacheManager.Get<MemoryCacheKeyObject>(key)
					.Convert(x => x != null && x.IsAdminKey);

		private bool IsAdminKey(string key)
			=> _cacheManager.Get<MemoryCacheKeyObject>(key)
					.Convert(x => x != null && x.IsAdminKey);

		private void SerializeSettings()
		{
			lock (_locker)
				File.WriteAllText(_settingsJsonFile, JsonConvert.SerializeObject(_settings));
		}

		private ConcurrentDictionary<string, ImageSettings> DeserializeSettings()
		{
			if (!File.Exists(_settingsJsonFile) || string.IsNullOrEmpty(File.ReadAllText(_settingsJsonFile)))
				return null;
			lock (_locker)
				return JsonConvert.DeserializeObject<ConcurrentDictionary<string, ImageSettings>>(
					File.ReadAllText(_settingsJsonFile));
		}
	}
}