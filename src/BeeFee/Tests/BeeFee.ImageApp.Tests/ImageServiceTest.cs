//using System.IO;
//using BeeFee.ImageApp.Services;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace BeeFee.ImageApp.Tests
//{
//    [TestClass]
//    public class ImageServiceTest
//    {
//		private ImageService _service;

//	    private const string TestImageName = "IMG_3946.jpg";

//	    [TestInitialize]
//	    public void Setup()
//	    {
//		    if (!Directory.Exists("images"))
//			    Directory.CreateDirectory("images");
//		    foreach (var directory in new DirectoryInfo("images").GetDirectories())
//			    directory.Delete(true);
//		    _service = new ImageService(@"images", @"public", @"originals", new ImageSize(2000, 2000), @"settings.json");

//		    _service.SetSetting("test", new[] { new ImageSize(200, 200), new ImageSize(400, 200) }, false);
//		}

//		private Stream GetTestImage(string filename) => File.OpenRead(filename);

//	    [TestMethod]
//	    public void AddImageTest()
//	    {
//		    ImageOperationResult operationResult;
//			using (var stream = GetTestImage(TestImageName))
//			{
//				operationResult = _service.AddImage(stream, "priroda.jpg", "test").Result;
//			}

//			Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//			Assert.AreEqual(null, operationResult.Error);
//			Assert.AreEqual("priroda.jpg", operationResult.Path);
//	    }


//		//Тест не актуален, изменяем имя файла до победного
//		//
//		//[TestMethod]
//		//public void AddExistingImageTest()
//		//{
//		//	ImageOperationResult result;
//		//	using (var stream = GetTestImage(TestImageName))
//		//	{
//		//		result = _service.AddImage(stream, "priroda.jpg", null).Result;
//		//	}

//		//	Assert.AreEqual(EImageOperationResult.Ok, result.Result);
//		//	Assert.AreEqual(null, result.Error);
//		//	Assert.AreEqual("priroda.jpg", result.Path);

//		//	using (var stream = GetTestImage(TestImageName))
//		//	{
//		//		result = _service.AddImage(stream, "priroda.jpg", null).Result;
//		//	}

//		//	Assert.AreEqual(EImageOperationResult.Exists, result.Result);
//		//	Assert.AreEqual(null, result.Error);
//		//	Assert.AreEqual("priroda.jpg", result.Path);
//		//}

//	    [TestMethod]
//	    public void AddImageWithSize()
//	    {
//			ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//			Assert.IsTrue(File.Exists("images/200x200/priroda.jpg"));
//			Assert.IsTrue(File.Exists("images/400x200/priroda.jpg"));
//		}

//	    [TestMethod]
//		public void GetOriginalImage()
//	    {
//			ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//			//Assert.AreEqual("/priroda.jpg" ,_service.GetImageUrl("priroda.jpg"));
//		}

//	    [TestMethod]
//	    public void GetResizedImage()
//	    {
//		    ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//		    //Assert.AreEqual("/200_200/priroda.jpg", _service.GetImageUrl(new ImageSize(200, 200), "priroda.jpg"));
//		    //Assert.AreEqual("/400_200/priroda.jpg", _service.GetImageUrl(new ImageSize(400, 200), "priroda.jpg"));
//	    }

//	    [TestMethod]
//	    public void GetNonexistentImage()
//	    {
//			ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//		    //Assert.AreEqual("", _service.GetImageUrl(new ImageSize(300, 200), "priroda.jpg"));

//		}

//	    [TestMethod]
//	    public void AddExistingImage()
//	    {
//		    ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);
//		}

//	    [TestMethod]
//	    public void RenameImageTest()
//	    {
//		    ImageOperationResult operationResult;
//			using(var stream = GetTestImage(TestImageName))
//			{
//				operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//					.Result;
//			}

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//		    var renameResult = _service.RenameImage("priroda.jpg", "test.jpg");

//			Assert.AreEqual(EImageOperationResult.Ok, renameResult.Result);
//			Assert.AreEqual(null, renameResult.Error);
//			Assert.AreEqual("test.jpg", renameResult.Path);
//	    }

//	    [TestMethod]
//	    public void RenameImageWithoutUnique()
//	    {
//			ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//			}

//		    using (var stream = GetTestImage(TestImageName))
//		    {
//				var result = 
//					_service.AddImage(stream, "test.jpg", "test").Result;
//			}

//			Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//		    var renameResult = _service.RenameImage("priroda.jpg", "test.jpg", false);

//		    Assert.AreEqual(EImageOperationResult.Error, renameResult.Result);
//		    Assert.AreEqual(EErrorType.FileAlreadyExists, renameResult.ErrorType);
//		    Assert.AreEqual("test.jpg", renameResult.Path);
//		}

//	    [TestMethod]
//	    public void UpdateImageTest()
//	    {
//		    ImageOperationResult operationResult;
//		    using (var stream = GetTestImage(TestImageName))
//		    {
//			    operationResult = _service.AddImage(stream, "priroda.jpg", "test")
//				    .Result;
//		    }

//		    Assert.AreEqual(EImageOperationResult.Ok, operationResult.Result);
//		    Assert.AreEqual(null, operationResult.Error);
//		    Assert.AreEqual("priroda.jpg", operationResult.Path);

//		    using (var stream = GetTestImage(TestImageName))
//		    {
//				_service.SetSetting("test2", new ImageSize[] {new ImageSize(600, 600)}, true);
//			    var updateResult = _service.UpdateImage(stream, "priroda.jpg", "test2").Result;

//				Assert.AreEqual(EImageOperationResult.Ok, updateResult.Result);
//				Assert.AreEqual(null, updateResult.Error);
//				Assert.AreEqual("priroda.jpg", updateResult.Path);
//		    }

//			Assert.IsTrue(Directory.Exists(Path.Combine("images", "600x600")));
//			Assert.IsTrue(File.Exists(Path.Combine("images", "public", "priroda.jpg")));
//	    }
//    }
//}
