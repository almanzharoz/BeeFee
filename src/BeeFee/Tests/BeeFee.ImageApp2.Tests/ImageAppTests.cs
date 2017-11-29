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
using SharpFuncExt;
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
				TimeForCachingKeys = 10,
				PreviewDirectory = "preview",
				PreviewSize = new Size(600, 600)
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

	    public static void GetFirstImage(Action<Stream> action)
		    => TestImageName.Using(File.OpenRead, (x, s) => action(s));

	    public static void GetSecondImage(Action<Stream> action)
		    => SecondImageName.Using(File.OpenRead, (x, s) => action(s));

	    public static void GetIcon(Action<Stream> action)
		    => "3d30b5kz.bmp".Using(File.OpenRead, (x, s) => action(s));

	    public static void GetNotImage(Action<Stream> action)
		    => "wdh.chm".Using(File.OpenRead, (x, s) => action(s));

	    [TestMethod]
	    public void AddImage()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    var img = _service.Add("test", "user", "123", x, "img.jpg");

			    Assert.IsTrue(File.Exists(img.TempPath));
			    Assert.IsTrue(File.Exists(img.PreviewPath));
		    });


	    [TestMethod]
	    public void AddImageTokenError()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    Assert.ThrowsException<AccessDeniedException>(() => _service.Add("test", "user", "1234", x, "img.jpg"));
		    });

	    [TestMethod]
	    public void AcceptFile()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    var img = _service.Add("test", "user", "123", x, "img.jpg");

			    var result = _service.AcceptFile(new List<ImageSettings>
			    {
				    //   new ImageSettings(img, "test/400_400/img.jpg", new Size(400,400)),
				    //new ImageSettings(img, "test/200_200/img.jpg", new Size(200, 200))
				    new ImageSettings(img.TempPath,
					    new ImageSaveSetting(new Size(400, 400), "test/400_400/img.jpg"),
					    new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg"))
			    }, "test");

			    Assert.IsTrue(result);
			    Assert.IsTrue(File.Exists("test/400_400/img.jpg"));
			    Assert.IsTrue(File.Exists("test/200_200/img.jpg"));
		    });

	    [TestMethod]
	    public void RemoveFile()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    var img = _service.Add("test", "user", "123", x, "img.jpg");

			    _service.AcceptFile(new List<ImageSettings>
			    {
				    //new ImageSettings(img, "test/img.jpg", new Size(300, 300))
				    new ImageSettings(img.TempPath,
					    new ImageSaveSetting(new Size(300, 300), "test/img.jpg"))
			    }, "test");

			    Assert.IsTrue(File.Exists(Path.Combine("test/img.jpg")));

			    _service.Remove("test", "user", "123", "img.jpg");

			    Assert.IsFalse(File.Exists(Path.Combine("test/img.jpg")));
		    });

	    [TestMethod]
	    public void RenameFile()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    var img = _service.Add("test", "user", "123", x, "img.jpg");

			    _service.AcceptFile(new List<ImageSettings>
			    {
				    //new ImageSettings(img, "test/img.jpg", new Size(300, 300))
				    new ImageSettings(img.TempPath,
					    new ImageSaveSetting(new Size(300, 300), "test/img.jpg"))
			    }, "test");

			    Assert.IsTrue(File.Exists(Path.Combine("test/img.jpg")));

			    _service.Rename("test", "user", "123", "img.jpg", "newImg.jpg");

			    Assert.IsFalse(File.Exists(Path.Combine("test/img.jpg")));
			    Assert.IsTrue(File.Exists(Path.Combine("test/newImg.jpg")));
		    });

	    [TestMethod]
	    public void OverrideFile()
		    => GetFirstImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");
			    var img = _service.Add("test", "user", "123", x, "img.jpg");

			    _service.AcceptFile(new[]
			    {
				    new ImageSettings(img.TempPath, new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")),
			    }, "test", true);

			    var firstDate = File.GetLastWriteTimeUtc("test/200_200/img.jpg");

			    GetSecondImage(y =>
			    {

				    var newImg = _service.Add("test", "user", "123", y, "img.jpg");
				    _service.AcceptFile(new[]
				    {
					    new ImageSettings(newImg.TempPath, new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")),
				    }, "test", true);

				    var secondDate = File.GetLastWriteTimeUtc("test/200_200/img.jpg");

				    Assert.AreNotEqual(firstDate, secondDate);
			    });
		    });

	    [TestMethod]
	    public void SmallSize()
		    => GetIcon(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");

			    Assert.ThrowsException<SizeTooSmallException>(() =>
				    _service.Add("test", "user", "123", x, "img.bmp"));
		    });

	    [TestMethod]
	    public void NotImage()
		    => GetNotImage(x =>
		    {
			    _service.GetAccess("test", "user", "123", "test");

			    Assert.ThrowsException<NotSupportedException>(() =>
				    _service.Add("test", "user", "123", x, "wdh.chm"));

		    });
    }
}
