using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp2.Tests
{
	public struct Struct1
	{
		public int Value1;
	}

	public struct Struct5
	{
		public int Value1;
		public int Value2;
		public int Value3;
		public int Value4;
		public int Value5;

		public int Sum()
		{
			return Value1 + Value2 + Value3 + Value4 + Value5;
		}
		public static int Sum(Struct5 s)
		{
			return s.Value1 + s.Value2 + s.Value3 + s.Value4 + s.Value5;
		}

		public static int Sum(ref Struct5 s)
		{
			return s.Value1 + s.Value2 + s.Value3 + s.Value4 + s.Value5;
		}

		public static int Sum(int v1, int v2, int v3, int v4, int v5)
		{
			return v1 + v2 + v3 + v4 + v5;
		}
	}

	[TestClass]
	public class DotNetStuctTests : DotNetTests
	{
		// 0,789860396767083
		[TestMethod]
		public void CreateStruct5Test()
			=> RunTest(nameof(CreateStruct5Test), CreateStruct5);

		public Stopwatch CreateStruct5()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Struct5
				{
					Value1 = i,
					Value2 = i,
					Value3 = i,
					Value4 = i,
					Value5 = i
				};
			sw.Stop();
			return sw;
		}

		// 0,0261606484893147
		[TestMethod]
		public void CreateStruct1WithConstTest()
			=> RunTest(nameof(CreateStruct1WithConstTest), CreateStruct1WithConst);

		public Stopwatch CreateStruct1WithConst()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Struct1
				{
					Value1 = 1,
				};
			sw.Stop();
			return sw;
		}

		// 0,1326378539493294
		[TestMethod]
		public void CreateStruct1Test()
			=> RunTest(nameof(CreateStruct1Test), CreateStruct1);

		public Stopwatch CreateStruct1()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Struct1
				{
					Value1 = i
				};
			sw.Stop();
			return sw;
		}

		// 0,3627889634601044
		[TestMethod]
		public void CreateStruct5WithConstTest()
			=> RunTest(nameof(CreateStruct5WithConstTest), CreateStruct5WithConst);

		public Stopwatch CreateStruct5WithConst()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Struct5
				{
					Value1 = 1,
					Value2 = 2,
					Value3 = 3,
					Value4 = 4,
					Value5 = 5
				};
			sw.Stop();
			return sw;
		}

		// 2,107810781078108
		[TestMethod]
		public void SumStructTest()
			=> RunTest(nameof(SumStructTest), SumStruct);

		public Stopwatch SumStruct()
		{
			var s = new Struct5
			{
				Value1 = 1,
				Value2 = 2,
				Value3 = 3,
				Value4 = 4,
				Value5 = 5
			};
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				s.Sum();
			sw.Stop();
			return sw;
		}

		// 2,41492098493201
		[TestMethod]
		public void StaticSumStructTest()
			=> RunTest(nameof(StaticSumStructTest), StaticSumStruct);

		public Stopwatch StaticSumStruct()
		{
			var s = new Struct5
			{
				Value1 = 1,
				Value2 = 2,
				Value3 = 3,
				Value4 = 4,
				Value5 = 5
			};
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Struct5.Sum(s);
			sw.Stop();
			return sw;
		}

		// 2,41492098493201
		[TestMethod]
		public void StaticSumRefStructTest()
			=> RunTest(nameof(StaticSumRefStructTest), StaticSumRefStruct);

		public Stopwatch StaticSumRefStruct()
		{
			var s = new Struct5
			{
				Value1 = 1,
				Value2 = 2,
				Value3 = 3,
				Value4 = 4,
				Value5 = 5
			};
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Struct5.Sum(ref s);
			sw.Stop();
			return sw;
		}

		// 2,41492098493201
		[TestMethod]
		public void StaticSumIntStructTest()
			=> RunTest(nameof(StaticSumIntStructTest), StaticSumIntStruct);

		public Stopwatch StaticSumIntStruct()
		{
			var s = new Struct5
			{
				Value1 = 1,
				Value2 = 2,
				Value3 = 3,
				Value4 = 4,
				Value5 = 5
			};
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Struct5.Sum(s.Value1, s.Value2, s.Value3, s.Value4, s.Value5);
			sw.Stop();
			return sw;
		}
	}
}