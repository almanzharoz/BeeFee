using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp.Tests
{
    [TestClass]
    public class ImageServiceTest
    {
		private ImageService _service;

	    private const string TestImageName = "IMG_3946.jpg";

	    [TestInitialize]
	    public void Setup()
	    {
		    if (!Directory.Exists("images"))
			    Directory.CreateDirectory("images");
		    foreach (var file in new DirectoryInfo("images").GetFiles())
			    file.Delete();
		    _service = new ImageService(@"images");
	    }

	    private Stream GetTestImage(string filename) => File.OpenRead(filename);

	    [TestMethod]
	    public void AddImageTest()
	    {
		    AddImageResult result;
			using (var stream = GetTestImage(TestImageName))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Ok, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);
	    }

		[TestMethod]
		public void AddExistingImageTest()
		{
			AddImageResult result;
			using (var stream = GetTestImage(TestImageName))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Ok, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);

			using (var stream = GetTestImage(TestImageName))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Exists, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);
		}

	    [TestMethod]
	    public void AddImageWithSize()
	    {
			AddImageResult result;
		    using (var stream = GetTestImage(TestImageName))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] {new ImageSize(200, 200), new ImageSize(400, 200)})
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

			Assert.IsTrue(File.Exists("images/200_200/priroda.jpg"));
			Assert.IsTrue(File.Exists("images/400_200/priroda.jpg"));
		}

	    [TestMethod]
		public void GetOriginalImage()
	    {
			AddImageResult result;
		    using (var stream = GetTestImage(TestImageName))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

			Assert.AreEqual("/priroda.jpg" ,_service.GetImageUrl("priroda.jpg"));
		}

	    [TestMethod]
	    public void GetResizedImage()
	    {
		    AddImageResult result;
		    using (var stream = GetTestImage(TestImageName))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

		    Assert.AreEqual("/200_200/priroda.jpg", _service.GetImageUrl(new ImageSize(200, 200), "priroda.jpg"));
		    Assert.AreEqual("/400_200/priroda.jpg", _service.GetImageUrl(new ImageSize(400, 200), "priroda.jpg"));
	    }

	    [TestMethod]
	    public void GetNonexistentImage()
	    {
			AddImageResult result;
		    using (var stream = GetTestImage(TestImageName))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

		    Assert.AreEqual("", _service.GetImageUrl(new ImageSize(300, 200), "priroda.jpg"));

		}
	}
}
