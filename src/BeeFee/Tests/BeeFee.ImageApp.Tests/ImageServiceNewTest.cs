using System;
using System.IO;
using System.Threading;
using BeeFee.ImageApp.Embed;
using BeeFee.ImageApp.Exceptions;
using BeeFee.ImageApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp.Tests
{
	[TestClass]
	public class ImageServiceNewTest
	{
		private ImageService _service;
		private const string TestImageName = "IMG_3946.JPG";
		private string _key;

		[TestInitialize]
		public void Setup()
		{
			if (!Directory.Exists("images"))
				Directory.CreateDirectory("images");
			foreach (var directory in new DirectoryInfo("images").GetDirectories())
			{
				directory.Delete(true);
			}
			_service = new ImageService("images", "original", "private", "resized", new ImageSize(2000, 2000), "settings.json", TimeSpan.FromSeconds(20));
			_service.SetSetting("test", new[] { new ImageSize(200, 200), new ImageSize(400, 200) }, false);
			_key = _service.RegisterEvent("testEvent");
		}

		private static Stream GetTestImage(string filename) => File.OpenRead(filename);

		[TestMethod]
		public void CreateDirectories()
		{
			Assert.IsTrue(Directory.Exists("images"));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "original")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "original", "testEvent")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "private")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "private", "testEvent")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "resized")));
			Assert.IsTrue(Directory.Exists(Path.Combine("images", "resized", "testEvent")));
		}

		[TestMethod]
		public void CreateExistingDirectories()
		{
			Assert.ThrowsException<DirectoryAlreadyExistsException>(() => _service.RegisterEvent("testEvent"));
		}

		[TestMethod]
		public void SimpleAddImageAndTestPath()
		{
			var img = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;

			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual(EImageOperationResult.Ok, img.Result);
		}

		[TestMethod]
		public void AddExistingImage()
		{
			var image1 = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", image1.Path);

			var image2 = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", image2.Path)));
		}

		[TestMethod]
		public void RenameImage()
		{
			var img = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.AreEqual("test.jpg", img.Path);

			var renamed = _service.RenameImage("testEvent", "test.jpg", "newname.jpg", _key);
			Assert.IsFalse(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "newname.jpg")));
			Assert.AreEqual("newname.jpg", renamed.Path);
		}

		[TestMethod]
		public void RenameToExistingImageWithChangingName()
		{
			var img = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;
			var img1 = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test1.jpg", "test", _key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test1.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual("test1.jpg", img1.Path);

			var renamed = _service.RenameImage("testEvent", "test.jpg", "test1.jpg", _key);
			Assert.IsFalse(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test10.jpg")));
			Assert.AreEqual("test10.jpg", renamed.Path);
		}

		[TestMethod]
		public void RenameToExistingImageWithoutChangingName()
		{
			var img = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test.jpg", "test", _key).Result;
			var img1 = _service.AddImage(GetTestImage(TestImageName), "testEvent", "test1.jpg", "test", _key).Result;
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test.jpg")));
			Assert.IsTrue(File.Exists(Path.Combine("images", "private", "testEvent", "test1.jpg")));
			Assert.AreEqual("test.jpg", img.Path);
			Assert.AreEqual("test1.jpg", img1.Path);

			var result = _service.RenameImage("testEvent", "test.jpg", "test1.jpg", _key, false);
			Assert.AreEqual(EErrorType.FileAlreadyExists, result.ErrorType);
			Assert.AreEqual(EImageOperationResult.Error, result.Result);

		}
	}
}