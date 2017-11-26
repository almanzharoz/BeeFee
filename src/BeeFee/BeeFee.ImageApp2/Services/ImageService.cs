﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp2.Embed;
using BeeFee.ImageApp2.Exceptions;
using SharpFuncExt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Helpers;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Services
{
	public class ImageService
	{
		private readonly ImageAppStartSettings _settings;
		private readonly MemoryCacheManager _cacheManager;

		public ImageService(ImageAppStartSettings settings, MemoryCacheManager cacheManager)
		{
			_settings = settings;
			_cacheManager = cacheManager;
		}

		/// <exception cref="FileNotSupportedException"></exception>
		/// <exception cref="SizeTooSmallException"></exception>
		public Task<string> Add(string directory, string ip, string token, Stream stream, string filename)
			=> new Task<string>(() => directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.Add),
				d => SaveFile(LoadImage(stream).Result,
					Path.Combine(_settings.TempDirectory, new Guid().ToString() + Path.GetExtension(filename))).Result,
				d => throw new AccessDeniedException()));

		public IEnumerable<string> GetListOfFiles(string directory, string ip, string token)
			=> directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.GetList),
				Directory.EnumerateFiles,
				d => throw new AccessDeniedException(d));

		public bool Remove(string directory, string ip, string token, string fileName)
			=> directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.Remove),
				d =>
				{
					File.Delete(Path.Combine(d, fileName));
					return true;
				},
				d => throw new AccessDeniedException(d));

		public bool Rename(string directory, string ip, string token, string oldFileName, string newFileName)
			=> directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.Rename),
				d =>
				{
					File.Move(Path.Combine(d, oldFileName), Path.Combine(d, newFileName));
					return true;
				},
				d => throw new AccessDeniedException());

		public bool GetAccess(string directory, string ip, string token, string requestIp)
		{
			if(!IsAdminIp(requestIp)) throw new AccessDeniedException();
			_cacheManager.Set(GetKey(directory, ip, token), 
				new MemoryCacheValueObject(EOperationType.Add, EOperationType.GetList, EOperationType.Remove, EOperationType.Rename),
				(int) _settings.TimeForCachingKeys.TotalMinutes);
			return true;
		}

		///<exception cref="SizeTooSmallException"></exception>
		public async Task<bool> AcceptFiles(IEnumerable<ImageSettings> settings, string requestIp)
		{
			if(!IsAdminIp(requestIp)) throw new AccessDeniedException();
			foreach (var file in settings)
			{
				var img = await LoadImage(File.OpenRead(file.TempPath));
				foreach (var size in file.Sizes)
				{
					SaveFile(ResizeImage(img, size), file.NewPath).Start();
				}
			}
			return true;
		}

		private bool IsAdminIp(string ip)
			=> _settings.AdminIps.Contains(ip);

		private static string GetKey(string directory, string ip, string token)
			=> directory + ip + token;

		private bool UserHasAccessToDirectory(string directory, string ip, string token, EOperationType type)
		{
			var value = _cacheManager.Get<MemoryCacheValueObject>(GetKey(directory, ip, token));
			return value != null && value.OperationTypes.Contains(type);
		}

		/// <exception cref="FileNotSupportedException"></exception>
		private static Task<string> SaveFile(Image<Rgba32> image, string path)
		{
			return new Task<string>(() =>
			{
				image.Save(path);
				return path;
			});
			
		}

		private static Image<Rgba32> ResizeImage(Image<Rgba32> image, Size size)
			=> image.Clone().Fluent(z =>
				z.Mutate(x => x.Resize(new ResizeOptions {Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height)})));

		/// <exception cref="SizeTooSmallException"></exception>
		private Task<Image<Rgba32>> LoadImage(Stream stream)
			=> new Task<Image<Rgba32>>(() => Image.Load(stream).If(
				img => img.Size().Width < _settings.MinimalSize.Width || img.Size().Height < _settings.MinimalSize.Height,
				img => throw new SizeTooSmallException(),
				img => ResizeImage(img, _settings.MaximalSize)));
	}
}