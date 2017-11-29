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
		/// <exception cref="NotSupportedException"></exception>
		public Task<(string TempPath, string PreviewPath)> Add(string directory, string ip, string token, Stream stream, string fileName)
			=> new Task<(string, string)>(() => directory.If(d => UserHasAccessToDirectory(d, ip, token, EOperationType.Add),
				d =>
				{
					using (var img = LoadImage(stream).Result)
					{
						var name = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(fileName));
						var temp = SaveFile(img, Path.Combine(_settings.TempDirectory, name)).Result;
						string preview;
						using(var resized = ResizeImage(img, _settings.PreviewSize))
							preview = SaveFile(resized, name).Result;
						return (temp, preview);
					}
				},
				d => throw new AccessDeniedException()));

		public (string TempPath, string PreviewPath) AddSynchronously(string directory, string ip, string token, Stream stream, string fileName)
			=> UserHasAccessToDirectory(directory, ip, token, EOperationType.Add).If(x => x, x =>
				{
					using (var img = LoadImageSynchronously(stream))
					{
						var name = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(fileName));
						var temp = SaveFileSynchronously(img, Path.Combine(_settings.TempDirectory, name));
						var preview = SaveFileSynchronously(img, Path.Combine(_settings.PreviewDirectory, name));
						return (temp, preview);
					}
				},
				x => throw new AccessDeniedException());

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
		public async Task<bool> AcceptFiles(IEnumerable<ImageSettings> settings, string requestIp, bool overrideExistingFiles = false)
		{
			if(!IsAdminIp(requestIp)) throw new AccessDeniedException();
			foreach (var setting in settings)
			{
				using (var img = await LoadImage(File.OpenRead(setting.TempPath)))
				{
					foreach (var newFile in setting.ImageSaveSettings)
					{
						if (!overrideExistingFiles && File.Exists(newFile.Path))
							continue;
						using(var resized = ResizeImage(img, newFile.Size))
							await SaveFile(resized, newFile.Path);
					}
				}
			}
			return true;
		}

		public bool AcceptFileSynchronously(IEnumerable<ImageSettings> settings, string requestIp, bool overrideExistingFiles = false)
		{
			if (!IsAdminIp(requestIp)) throw new AccessDeniedException();
			foreach (var setting in settings)
			{
				using (var img = LoadImageSynchronously(File.OpenRead(setting.TempPath)))
				{
					foreach (var newFile in setting.ImageSaveSettings)
					{
						if (!overrideExistingFiles && File.Exists(newFile.Path))
							continue;
						using(var resized = ResizeImage(img, newFile.Size))
							SaveFileSynchronously(resized, newFile.Path);
					}
				}
			}
			return true;
		}

		private bool IsAdminIp(string ip)
			=> _settings.AdminHosts.Contains(ip);

		private static string GetKey(string directory, string ip, string token)
			=> string.Concat(directory.HasNotNullArg(nameof(directory)), ip.HasNotNullArg(nameof(ip)), token.HasNotNullArg(nameof(token)));

		private bool UserHasAccessToDirectory(string directory, string ip, string token, EOperationType type)
			=> _cacheManager.Get<MemoryCacheValueObject>(GetKey(directory, ip, token))
				.IfNotNull(x => x.OperationTypes.Contains(type), () => false);

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
			=> path.HasNotNullArg(nameof(path))
				.Fluent(x => x.IfNot(p => Directory.Exists(Path.GetDirectoryName(p)), p => Directory.CreateDirectory(Path.GetDirectoryName(p))))
				.Fluent(x => image.Save(x));

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
			=> stream.Using(Image.Load,
				(s, img) => ResizeImage(
					img.ThrowIf(x => x.Size().Width < _settings.MinimalSize.Width || x.Size().Height < _settings.MinimalSize.Height,
						x => new SizeTooSmallException()), _settings.MaximalSize));
	}
}