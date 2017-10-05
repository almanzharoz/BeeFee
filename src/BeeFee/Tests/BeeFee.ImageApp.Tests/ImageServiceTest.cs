using System;
using System.IO;
using System.Threading.Tasks;
using BeeFee.ImageApp.Caching;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp.Tests
{
	[TestClass]
	public class ImageServiceTest
	{
		private ImageService _service;
		private const string TestImageName = "IMG_3946.JPG";
		private const string SecondImageName = "pochemu-samolety-letaut4.jpg";
		private const string Key = "123.123.123.123";

		[TestInitialize]
		public void Setup()
		{
			if (!Directory.Exists("images"))
				Directory.CreateDirectory("images");
			foreach (var directory in new DirectoryInfo("images").GetDirectories())
			{
				DeleteDirectory(directory.FullName);
			}

			var pathHandler = new PathHandler("images", "private", "public", "resized", "users", "companies", "avatar.jpg", "logo.jpg");

			_service = new ImageService(new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions())), "settings.json", new ImageSize(200, 200), 
				new ImageSize(200, 200), new ImageSize(4000, 2000), pathHandler, 20, 20);
			_service.SetSetting("test", new ImageSettings(new[] { new ImageSize(200, 200), new ImageSize(400, 200) }, false, true), Key);
			_service.RegisterEvent("testCompany", "testEvent", Key);
			_service.GetAccessToFolder(Key, "testCompany");
			_service.GetAccessToFolder(Key, "testUser");
		}

		private static void DeleteDirectory(string path, int i = 0)
		{
			if (i == 10) return;
			foreach (var directory in Directory.GetDirectories(path))
			{
				DeleteDirectory(directory);
			}
			try
			{
				Directory.Delete(path, true);
			}
			catch (IOException)
			{
				DeleteDirectory(path, ++i);
			}
			catch (UnauthorizedAccessException)
			{
				DeleteDirectory(path, ++i);
			}
		}

		private static Stream GetTestImage(string filename) => File.OpenRead(filename);

		[TestMethod]
		public void CreateDirectories()
		{
			Assert.IsTrue(Directory.Exists("images"));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "private")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "private", "testCompany")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "private", "testCompany", "testEvent")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "public")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "public", "testCompany")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "public", "testCompany", "testEvent")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "resized")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "resized", "testCompany")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "resized", "testCompany", "testEvent")));
		}

		[TestMethod]
		public void CreateExistingDirectories()
		{
			Assert.ThrowsException<DirectoryAlreadyExistsException>(() => _service.RegisterEvent("testCompany", "testEvent", Key));
		}

		[TestMethod]
		public void SimpleAddImageAndTestPath()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;

			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual(EImageOperationResult.Ok, img.Result);
		}

		[TestMethod]
		public void AddExistingImage()
		{
			var image1 = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", image1.Path);

			var image2 = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", image2.Path)));
		}

		[TestMethod]
		public void RenameImage()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);

			_service.RenameEventImage("testCompany", "testEvent", "test.jpg", "newname.jpg", true, Key);
			Assert.IsFalse(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "newname.jpg")));
		}

		[TestMethod]
		public void RenameToExistingImageWithChangingName()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			var img1 = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test1.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test1.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual("test1.jpg", img1.Path);

			_service.RenameEventImage("testCompany", "testEvent", "test.jpg", "test1.jpg", true, Key);
			Assert.IsFalse(File.Exists(Path.Combine("images", "private","testCompany", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test1.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test10.jpg")));
		}

		[TestMethod]
		public void RenameToExistingImageWithoutChangingName()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			var img1 = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test1.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test1.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual("test1.jpg", img1.Path);

			var result = _service.RenameEventImage("testCompany", "testEvent", "test.jpg", "test1.jpg", false, Key);
			Assert.AreEqual(EErrorType.FileAlreadyExists, result.ErrorType);
			Assert.AreEqual(EImageOperationResult.Error, result.Result);
		}

		[TestMethod]
		public void UpdateFile()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			var created = File.GetLastWriteTimeUtc(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg"));

			var updated = _service.UpdateEventImage(GetTestImage(SecondImageName),"testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.AreNotEqual(created, File.GetLastWriteTimeUtc(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual(EImageOperationResult.Ok, updated.Result);
		}

		[TestMethod]
		public void RemoveImage()
		{
			var img = _service.AddEventImage(GetTestImage(TestImageName), "testCompany", "testEvent", "test.jpg", "test", Key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testCompany", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);

			_service.RemoveEventImage("testCompany", "testEvent", "test.jpg", Key);
			Assert.IsFalse(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
		}

		[TestMethod]
		public void AddUserAvatar()
		{
			Task.Run(() => _service.AddUserAvatar(GetTestImage(TestImageName), "testUser", Key)).Wait();
			Assert.IsTrue(File.Exists(Path.Combine("images", "public", "users", "testUser", "avatar.jpg")));
		}

		[TestMethod]
		public void AddCompanyLogo()
		{
			Task.Run(() => _service.AddCompanyLogo(GetTestImage(TestImageName), "testCompany", Key)).Wait();
			Assert.IsTrue(File.Exists(Path.Combine("images", "public", "companies", "testCompany", "logo.jpg")));
		}
	}
}