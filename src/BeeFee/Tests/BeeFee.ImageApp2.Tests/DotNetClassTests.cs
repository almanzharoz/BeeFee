using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp2.Tests
{
	public class Class1
	{
		public int Value1;
	}

	public class Class5
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
		public static int Sum(Class5 s)
		{
			return s.Value1 + s.Value2 + s.Value3 + s.Value4 + s.Value5;
		}

		public static int Sum(ref Class5 s)
		{
			return s.Value1 + s.Value2 + s.Value3 + s.Value4 + s.Value5;
		}

		public static int Sum(int v1, int v2, int v3, int v4, int v5)
		{
			return v1 + v2 + v3 + v4 + v5;
		}
	}

	[TestClass]
	public class DotNetClassTests : DotNetTests
	{
		// 0,789860396767083
		[TestMethod]
		public void CreateClass5Test()
			=> RunTest(nameof(CreateClass5Test), CreateClass5);

		public Stopwatch CreateClass5()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Class5
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
		public void CreateClass1WithConstTest()
			=> RunTest(nameof(CreateClass1WithConstTest), CreateClass1WithConst);

		public Stopwatch CreateClass1WithConst()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Class1
				{
					Value1 = 1,
				};
			sw.Stop();
			return sw;
		}

		// 0,1326378539493294
		[TestMethod]
		public void CreateClass1Test()
			=> RunTest(nameof(CreateClass1Test), CreateClass1);

		public Stopwatch CreateClass1()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Class1
				{
					Value1 = i
				};
			sw.Stop();
			return sw;
		}

		// 0,3627889634601044
		[TestMethod]
		public void CreateClass5WithConstTest()
			=> RunTest(nameof(CreateClass5WithConstTest), CreateClass5WithConst);

		public Stopwatch CreateClass5WithConst()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new Class5
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
			var s = new Class5
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
			var s = new Class5
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
				Class5.Sum(s);
			sw.Stop();
			return sw;
		}

		// 2,41492098493201
		[TestMethod]
		public void StaticSumRefStructTest()
			=> RunTest(nameof(StaticSumRefStructTest), StaticSumRefStruct);

		public Stopwatch StaticSumRefStruct()
		{
			var s = new Class5
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
				Class5.Sum(ref s);
			sw.Stop();
			return sw;
		}

		// 2,41492098493201
		[TestMethod]
		public void StaticSumIntStructTest()
			=> RunTest(nameof(StaticSumIntStructTest), StaticSumIntStruct);

		public Stopwatch StaticSumIntStruct()
		{
			var s = new Class5
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
				Class5.Sum(s.Value1, s.Value2, s.Value3, s.Value4, s.Value5);
			sw.Stop();
			return sw;
		}
	}
}
