using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BeeFee.ImageApp2.Caching;
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
			Directory.CreateDirectory(settings.TempDirectory);
		}

		/// <exception cref="FileNotSupportedException"></exception>
		/// <exception cref="SizeTooSmallException"></exception>
		public Task<string> Add(string directory, string ip, string token, Stream stream, string filename)
			=> new Task<string>(() => directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.Add),
				d => SaveFile(LoadImage(stream).Result,
					Path.Combine(_settings.TempDirectory, string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(filename)))).Result,
				d => throw new AccessDeniedException()));

		public string AddSynchronously(string directory, string ip, string token, Stream stream, string fileName)
		{
			if (UserHasAccessToDirectory(directory, ip, token, EOperationType.Add))
				return SaveFileSynchronously(LoadImageSynchronously(stream),
					Path.Combine(_settings.TempDirectory, string.Concat(Guid.NewGuid(), Path.GetExtension(fileName))));
			throw new AccessDeniedException();
		}

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
			if (!IsAdminIp(requestIp)) throw new AccessDeniedException();
			_cacheManager.Set(GetKey(directory, ip, token),
				new MemoryCacheValueObject(EOperationType.Add, EOperationType.GetList, EOperationType.Remove,
					EOperationType.Rename),
				_settings.TimeForCachingKeys);
			return true;
		}

		///<exception cref="SizeTooSmallException"></exception>
		public async Task<bool> AcceptFiles(IEnumerable<ImageSettings> settings, string requestIp)
		{
			if(!IsAdminIp(requestIp)) throw new AccessDeniedException();
			foreach (var file in settings)
			{
				if (File.Exists(file.NewPath)) return false;
				var img = await LoadImage(File.OpenRead(file.TempPath));
				await SaveFile(ResizeImage(img, file.Size), file.NewPath);
			}
			return true;
		}

		public bool AcceptFileSynchronously(IEnumerable<ImageSettings> settings, string requestIp)
		{
			if (!IsAdminIp(requestIp)) throw new AccessDeniedException();
			foreach (var file in settings)
			{
				if (File.Exists(file.NewPath)) return false;
				var img = LoadImageSynchronously(File.OpenRead(file.TempPath));
				SaveFileSynchronously(ResizeImage(img, file.Size), file.NewPath);
			}
			return true;
		}

		private bool IsAdminIp(string ip)
			=> _settings.AdminHosts.Contains(ip);

		private static string GetKey(string directory, string ip, string token)
			=> string.Concat(directory.HasNotNullArg(nameof(directory)), ip.HasNotNullArg(nameof(ip)), token.HasNotNullArg(nameof(token)));

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
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				image.Save(path);
				return path;
			}); // TODO: Добавить обработку ошибок в асинхронном режиме
			
		}

		private static string SaveFileSynchronously(Image<Rgba32> image, string path)
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			image.Save(path);
			return path;
		}

		private static Image<Rgba32> ResizeImage(Image<Rgba32> image, Size size)
			=> image.Clone().Fluent(z =>
				z.Mutate(x => x.Resize(new ResizeOptions {Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height)})));

		/// <exception cref="SizeTooSmallException"></exception>
		private Task<Image<Rgba32>> LoadImage(Stream stream)
			=> new Task<Image<Rgba32>>(() => Image.Load(stream).If(
				img => img.Size().Width < _settings.MinimalSize.Width || img.Size().Height < _settings.MinimalSize.Height,
				img => throw new SizeTooSmallException(),
				img => ResizeImage(img, _settings.MaximalSize)));// TODO: Добавить обработку ошибок в асинхронном режиме

		private Image<Rgba32> LoadImageSynchronously(Stream stream)
		{
			var img = Image.Load(stream);
			if(img.Size().Width < _settings.MinimalSize.Width || img.Size().Height < _settings.MinimalSize.Height)
				throw new SizeTooSmallException();
			stream.Dispose();
			return ResizeImage(img, _settings.MaximalSize);
		}
	}
}