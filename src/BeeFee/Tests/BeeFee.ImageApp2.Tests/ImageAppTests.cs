using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeeFee.ImageApp2.Caching;
using BeeFee.ImageApp2.Embed;
using BeeFee.ImageApp2.Exceptions;
using BeeFee.ImageApp2.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.Primitives;

namespace BeeFee.ImageApp2.Tests
{
    [TestClass]
    public class ImageAppTests
    {
	    private ImageService _service;
	    private const string TestImageName = "IMG_3946.JPG";
	    private const string SecondImageName = "pochemu-samolety-letaut4.jpg";

	    [TestInitialize]
	    public void Setup()
	    {
		    var settings = new ImageAppStartSettings
		    {
			    AdminHosts = new List<string> { "test" },
				MaximalSize = new Size(2000, 2000),
				MinimalSize = new Size(32, 32),
				TempDirectory = "temp",
				TimeForCachingKeys = 10
		    };

		    _service = new ImageService(settings, new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions())));
	    }

	    [TestCleanup]
	    public void Cleanup()
	    {
		    try
		    {
				Directory.Delete("temp", true);
		    }
		    catch (DirectoryNotFoundException)
		    {
		    }

		    try
		    {
			    Directory.Delete("test", true);
			}
			catch (DirectoryNotFoundException)
			{ 
		    }
	    }

	    public static FileStream GetFirstImage()
		    => File.OpenRead(TestImageName);

	    public static FileStream GetSecondImage()
		    => File.OpenRead(SecondImageName);

	    public static FileStream GetIcon()
		    => File.OpenRead("3d30b5kz.bmp");

	    public static FileStream GetNotImage()
		    => File.OpenRead("wdh.chm");

        [TestMethod]
        public void AddImage()
        {
	        _service.GetAccess("test", "user", "123", "test");
	        var img = _service.AddSynchronously("test", "user", "123", GetFirstImage(), "img.jpg");

			Assert.IsTrue(File.Exists(img));
        }

	    [TestMethod]
	    public void AddImageTokenError()
	    {
		    _service.GetAccess("test", "user", "123", "test");
		    Assert.ThrowsException<AccessDeniedException>(()=>_service.AddSynchronously("test", "user", "1234", GetFirstImage(), "img.jpg"));
	    }

		[TestMethod]
	    public void AcceptFile()
	    {
			_service.GetAccess("test", "user", "123", "test");
		    var img = _service.AddSynchronously("test", "user", "123", GetFirstImage(), "img.jpg");

		    var result = _service.AcceptFileSynchronously(new List<ImageSettings>
		    {
			    //   new ImageSettings(img, "test/400_400/img.jpg", new Size(400,400)),
			    //new ImageSettings(img, "test/200_200/img.jpg", new Size(200, 200))
			    new ImageSettings(img,
				    new ImageSaveSetting(new Size(400, 400), "test/400_400/img.jpg"),
				    new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg"))
		    }, "test");

			Assert.IsTrue(result);
			Assert.IsTrue(File.Exists("test/400_400/img.jpg"));
			Assert.IsTrue(File.Exists("test/200_200/img.jpg"));
	    }

	    [TestMethod]
	    public void RemoveFile()
	    {
			_service.GetAccess("test", "user", "123", "test");
		    var img = _service.AddSynchronously("test", "user", "123", GetFirstImage(), "img.jpg");

		    _service.AcceptFileSynchronously(new List<ImageSettings>
		    {
			    //new ImageSettings(img, "test/img.jpg", new Size(300, 300))
			    new ImageSettings(img,
				    new ImageSaveSetting(new Size(300, 300), "test/img.jpg"))
		    }, "test");

			Assert.IsTrue(File.Exists(Path.Combine("test/img.jpg")));

			_service.Remove("test", "user", "123", "img.jpg");

			Assert.IsFalse(File.Exists(Path.Combine("test/img.jpg")));
	    }

	    [TestMethod]
	    public void RenameFile()
	    {
			_service.GetAccess("test", "user", "123", "test");
		    var img = _service.AddSynchronously("test", "user", "123", GetFirstImage(), "img.jpg");

		    _service.AcceptFileSynchronously(new List<ImageSettings>
		    {
			    //new ImageSettings(img, "test/img.jpg", new Size(300, 300))
			    new ImageSettings(img,
				    new ImageSaveSetting(new Size(300, 300), "test/img.jpg"))
		    }, "test");

		    Assert.IsTrue(File.Exists(Path.Combine("test/img.jpg")));

		    _service.Rename("test", "user", "123", "img.jpg", "newImg.jpg");

		    Assert.IsFalse(File.Exists(Path.Combine("test/img.jpg")));
		    Assert.IsTrue(File.Exists(Path.Combine("test/newImg.jpg")));
		}

	    [TestMethod]
	    public void OverrideFile()
	    {
			_service.GetAccess("test", "user", "", "test");
		    var img = _service.AddSynchronously("test", "user", "", GetFirstImage(), "img.jpg");

		    _service.AcceptFileSynchronously(new[]
		    {
				new ImageSettings(img, new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")), 
		    }, "test", true);

		    var firstDate = File.GetLastWriteTimeUtc("test/200_200/img.jpg");

		    var newImg = _service.AddSynchronously("test", "user", "", GetSecondImage(), "img.jpg");
		    _service.AcceptFileSynchronously(new[]
		    {
			    new ImageSettings(newImg, new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")),
		    }, "test", true);

		    var secondDate = File.GetLastWriteTimeUtc("test/200_200/img.jpg");

			Assert.AreNotEqual(firstDate, secondDate);
	    }

	    [TestMethod]
	    public void SmallSize()
	    {
			_service.GetAccess("test", "user", "", "test");

		    Assert.ThrowsException<SizeTooSmallException>(() =>
			    _service.AddSynchronously("test", "user", "", GetIcon(), "img.bmp"));
	    }

	    [TestMethod]
	    public void NotImage()
	    {
			_service.GetAccess("test", "user", "", "test");

		    Assert.ThrowsException<NotSupportedException>(() =>
			    _service.AddSynchronously("test", "user", "", GetNotImage(), "wdh.chm"));

	    }
	}
}
