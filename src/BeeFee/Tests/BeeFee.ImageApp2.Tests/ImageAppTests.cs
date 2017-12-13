using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BeeFee.ImageApp2.Caching;
using BeeFee.ImageApp2.Embed;
using BeeFee.ImageApp2.Exceptions;
using BeeFee.ImageApp2.Services;
using Core.ElasticSearch.Serialization;
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
				AdminHosts = new List<string> {"test"},
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

				Assert.IsTrue(File.Exists(Path.Combine("temp", img)));
				Assert.IsTrue(File.Exists(Path.Combine("preview", img)));
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
					new ImageSettings(Path.Combine("temp", img),
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
					new ImageSettings(Path.Combine("temp", img),
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
					new ImageSettings(Path.Combine("temp", img),
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
					new ImageSettings(Path.Combine("temp", img), new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")),
				}, "test", true);

				var firstDate = File.GetLastWriteTimeUtc("test/200_200/img.jpg");

				GetSecondImage(y =>
				{

					var newImg = _service.Add("test", "user", "123", y, "img.jpg");
					_service.AcceptFile(new[]
					{
						new ImageSettings(Path.Combine("temp", img), new ImageSaveSetting(new Size(200, 200), "test/200_200/img.jpg")),
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

		private static int GetElement(int[] array, int index) => array[index];

		[TestMethod]
		public void ArrayTest()
		{
			var a = new int[10000000];
			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < a.Length; i++)
				a[i] = i;
			sw.Stop();
			Console.WriteLine("Write: " + sw.ElapsedMilliseconds);

			sw.Restart();
			var j = 0;
			for (var i = 0; i < a.Length; i++)
				j += a[i];
			sw.Stop();
			Console.WriteLine("Read: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			j = 0;
			foreach (var i in a)
				j += i;
			sw.Stop();
			Console.WriteLine("Each: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			j = 0;
			IEnumerable<int> b = a;
			var c = b.GetEnumerator();
			sw.Restart();
			foreach (var i in b)
				j += i;
			sw.Stop();
			Console.WriteLine("Each (IE): " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			j = 0;
			sw.Restart();
			while (c.MoveNext())
				j += c.Current;
			sw.Stop();
			Console.WriteLine("Each (MoveNext): " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
			c.Dispose();

			j = 0;
			sw.Restart();
			for (var i = 0; i < a.Length; i++)
				j += GetElement(a, i);
			sw.Stop();
			Console.WriteLine("Each (GetElement): " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			Console.WriteLine(j + ", " + a.Length);
		}

		[TestMethod]
		public void ListTest()
		{
			var a = new List<int>();
			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < 1000000; i++)
				a.Add(i);
			sw.Stop();
			Console.WriteLine("Add: " + sw.ElapsedMilliseconds);

			sw.Restart();
			for (var i = 0; i < 1000000; i++)
				a[i] = i;
			sw.Stop();
			Console.WriteLine("Write: " + sw.ElapsedMilliseconds);

			sw.Restart();
			var j = 0;
			for (var i = 0; i < 1000000; i++)
				j += a[i];
			sw.Stop();
			Console.WriteLine("Read: " + sw.ElapsedMilliseconds);

			sw.Restart();
			j = 0;
			foreach (var i in a)
				j += i;
			sw.Stop();
			Console.WriteLine("Each: " + sw.ElapsedMilliseconds);

			sw.Restart();
			j = 0;
			IEnumerable<int> b = a;
			foreach (var i in b)
				j += i;
			sw.Stop();
			Console.WriteLine("Each (IE): " + sw.ElapsedMilliseconds);

			Console.WriteLine(j + ", " + a.Count);
		}

		[TestMethod]
		public void CreateTest()
		{
			TestStruct a;
			TestClass c;
			TestStructField af;
			TestClassField cf;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < 1000000; i++)
				a = new TestStruct(i);
			sw.Stop();
			Console.WriteLine("Struct with constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				a = new TestStruct() { Value = i };
			sw.Stop();
			Console.WriteLine("Struct without constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = new TestClass(i);
			sw.Stop();
			Console.WriteLine("Class with constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = new TestClass() { Value = i };
			sw.Stop();
			Console.WriteLine("Class without constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				af = new TestStructField(i);
			sw.Stop();
			Console.WriteLine("StructField with constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				af = new TestStructField() {Value = i};
			sw.Stop();
			Console.WriteLine("StructField without constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				cf = new TestClassField(i);
			sw.Stop();
			Console.WriteLine("ClassField with constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				cf = new TestClassField() {Value = i};
			sw.Stop();
			Console.WriteLine("ClassField without constructor: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
		}

		[TestMethod]
		public void CreateActivatorTest()
		{
			var st = typeof(TestStruct);
			var sctor = st.GetConstructors().First(x => x.GetParameters().Any());
			var sactivator = ObjectActivator.GetActivator<TestStruct>(sctor);

			var ct = typeof(TestClass);
			var cdctor = ct.GetConstructors().First(x => !x.GetParameters().Any());
			var cdactivator = ObjectActivator.GetActivator<TestClass>(cdctor);
			var cctor = ct.GetConstructors().First(x => x.GetParameters().Any());
			var cactivator = ObjectActivator.GetActivator<TestClass>(cdctor);

			TestStruct a;
			TestClass c;
			TestStructField af;
			TestClassField cf;
			var sw = new Stopwatch();

			sw.Start();
			for (int i = 0; i < 1000000; i++)
				a = new TestStruct(i);
			sw.Stop();
			Console.WriteLine("Struct with new: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				a = sactivator.Creator(i);
			sw.Stop();
			Console.WriteLine("Struct with ObjectActivator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				a = (TestStruct)Activator.CreateInstance(st, new object[]{i}, null);
			sw.Stop();
			Console.WriteLine("Struct with Activator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = new TestClass();
			sw.Stop();
			Console.WriteLine("ClassDefault with new: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = cdactivator.Creator();
			sw.Stop();
			Console.WriteLine("ClassDefault with ObjectActivator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = (TestClass)Activator.CreateInstance(ct, null, null);
			sw.Stop();
			Console.WriteLine("ClassDefault with Activator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = new TestClass(i);
			sw.Stop();
			Console.WriteLine("Class with new: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = cactivator.Creator(i);
			sw.Stop();
			Console.WriteLine("Class with ObjectActivator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (int i = 0; i < 1000000; i++)
				c = (TestClass)Activator.CreateInstance(ct, new object[] { i }, null);
			sw.Stop();
			Console.WriteLine("Class with Activator: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
		}

		[TestMethod]
		public void FuncTest()
		{
			var s = new TestStructField(10);
			var c = new TestClassField(10);
			var sw = new Stopwatch();

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				Func(s);
			sw.Stop();
			Console.WriteLine("Func with const struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				Func(new TestStructField(){Value = i});
			sw.Stop();
			Console.WriteLine("Func with struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				Func(c);
			sw.Stop();
			Console.WriteLine("Func with const class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				Func(new TestClassField() { Value = i });
			sw.Stop();
			Console.WriteLine("Func with class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				Func(i);
			sw.Stop();
			Console.WriteLine("Func with int: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				Func("123");
			sw.Stop();
			Console.WriteLine("Func with const string: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				Func();
			sw.Stop();
			Console.WriteLine("Func without param: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 1000000; i++) ;
			sw.Stop();
			Console.WriteLine("Empty for: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
		}

		[TestMethod]
		public void StructFuncTest()
		{
			var f = new TestStruct();
			var s = new TestStructField(10);
			var c = new TestClassField(10);
			var sw = new Stopwatch();

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				f.Func(s);
			sw.Stop();
			Console.WriteLine("Func with const struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(new TestStructField() { Value = i });
			sw.Stop();
			Console.WriteLine("Func with struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				f.Func(c);
			sw.Stop();
			Console.WriteLine("Func with const class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(new TestClassField() { Value = i });
			sw.Stop();
			Console.WriteLine("Func with class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(i);
			sw.Stop();
			Console.WriteLine("Func with int: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func("123");
			sw.Stop();
			Console.WriteLine("Func with const string: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func();
			sw.Stop();
			Console.WriteLine("Func without param: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 1000000; i++) ;
			sw.Stop();
			Console.WriteLine("Empty for: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
		}

		[TestMethod]
		public void ClassFuncTest()
		{
			var f = new TestClass();
			var s = new TestStructField(10);
			var c = new TestClassField(10);
			var sw = new Stopwatch();

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				f.Func(s);
			sw.Stop();
			Console.WriteLine("Func with const struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(new TestStructField() { Value = i });
			sw.Stop();
			Console.WriteLine("Func with struct: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Start();
			for (var i = 0; i < 10000000; i++)
				f.Func(c);
			sw.Stop();
			Console.WriteLine("Func with const class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(new TestClassField() { Value = i });
			sw.Stop();
			Console.WriteLine("Func with class: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func(i);
			sw.Stop();
			Console.WriteLine("Func with int: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func("123");
			sw.Stop();
			Console.WriteLine("Func with const string: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 10000000; i++)
				f.Func();
			sw.Stop();
			Console.WriteLine("Func without param: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);

			sw.Restart();
			for (var i = 0; i < 1000000; i++) ;
			sw.Stop();
			Console.WriteLine("Empty for: " + sw.ElapsedMilliseconds + " Ticks: " + sw.ElapsedTicks);
		}

		private void Func(TestClassField s) {}
		private void Func(TestStructField s) {}
		private void Func(int i) {}
		private void Func(string s) {}
		private void Func() {}

	}

	public struct TestStruct
	{
		public int Value { get; set; }

		public TestStruct(int value)
		{
			Value = value;
		}

		public void Func(TestClassField s) { }
		public void Func(TestStructField s) { }
		public void Func(int i) { }
		public void Func(string s) { }
		public void Func() { }

	}
	public class TestClass
	{
		public int Value { get; set; }

		public TestClass() { }
		public TestClass(int value)
		{
			Value = value;
		}

		public void Func(TestClassField s) { }
		public void Func(TestStructField s) { }
		public void Func(int i) { }
		public void Func(string s) { }
		public void Func() { }

	}

	public struct TestStructField
	{
		public int Value;

		public TestStructField(int value)
		{
			Value = value;
		}
	}
	public class TestClassField
	{
		public int Value;

		public TestClassField() { }
		public TestClassField(int value)
		{
			Value = value;
		}
	}

}
