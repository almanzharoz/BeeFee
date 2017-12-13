using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp2.Tests
{
	public struct ReturnStruct
	{
		public int Value;
	}
	public class ReturnClass
	{
		public int Value;
	}

	public struct StructFunc
	{
		public static ReturnStruct ReturnStruct = new ReturnStruct() {Value = 1};
		public static ReturnClass ReturnClass = new ReturnClass() {Value = 1};

		public void Action(ReturnStruct s) { }
		public void Action(ReturnClass s) { }
		public void Action(int i) { }
		public void Action(string s) { }
		public void Action() { }

		public int FuncInt(ReturnStruct s) => 1;
		public int FuncInt(ReturnClass s) => 1;
		public int FuncInt(int i) => 1;
		public int FuncInt(string s) => 1;
		public int FuncInt() => 1;

		public ReturnStruct FuncStruct(ReturnStruct s) => ReturnStruct;
		public ReturnStruct FuncStruct(ReturnClass s) => ReturnStruct;
		public ReturnStruct FuncStruct(int i) => ReturnStruct;
		public ReturnStruct FuncStruct(string s) => ReturnStruct;
		public ReturnStruct FuncStruct() => ReturnStruct;

		public ReturnClass FuncClass(ReturnClass s) => ReturnClass;
		public ReturnClass FuncClass(ReturnStruct s) => ReturnClass;
		public ReturnClass FuncClass(int i) => ReturnClass;
		public ReturnClass FuncClass(string s) => ReturnClass;
		public ReturnClass FuncClass() => ReturnClass;
	}

	public class ClassFunc
	{
		public static ReturnStruct ReturnStruct = new ReturnStruct() { Value = 1 };
		public static ReturnClass ReturnClass = new ReturnClass() { Value = 1 };

		public void Action(ReturnStruct s) { }
		public void Action(ReturnClass s) { }
		public void Action(int i) { }
		public void Action(string s) { }
		public void Action() { }

		public int FuncInt(ReturnStruct s) => 1;
		public int FuncInt(ReturnClass s) => 1;
		public int FuncInt(int i) => 1;
		public int FuncInt(string s) => 1;
		public int FuncInt() => 1;

		public ReturnStruct FuncStruct(ReturnStruct s) => ReturnStruct;
		public ReturnStruct FuncStruct(ReturnClass s) => ReturnStruct;
		public ReturnStruct FuncStruct(int i) => ReturnStruct;
		public ReturnStruct FuncStruct(string s) => ReturnStruct;
		public ReturnStruct FuncStruct() => ReturnStruct;

		public ReturnClass FuncClass(ReturnStruct s) => ReturnClass;
		public ReturnClass FuncClass(ReturnClass s) => ReturnClass;
		public ReturnClass FuncClass(int i) => ReturnClass;
		public ReturnClass FuncClass(string s) => ReturnClass;
		public ReturnClass FuncClass() => ReturnClass;
	}

	public static class StaticClass
	{
		public static ReturnStruct ReturnStruct = new ReturnStruct() { Value = 1 };
		public static ReturnClass ReturnClass = new ReturnClass() { Value = 1 };

		public static void Action(ReturnStruct s) { }
		public static void Action(ReturnClass s) { }
		public static void Action(int i) { }
		public static void Action(string s) { }
		public static void Action() { }

		public static int FuncInt(ReturnStruct s) => 1;
		public static int FuncInt(ReturnClass s) => 1;
		public static int FuncInt(int i) => 1;
		public static int FuncInt(string s) => 1;
		public static int FuncInt() => 1;

		public static ReturnStruct FuncStruct(ReturnStruct s) => ReturnStruct;
		public static ReturnStruct FuncStruct(ReturnClass s) => ReturnStruct;
		public static ReturnStruct FuncStruct(int i) => ReturnStruct;
		public static ReturnStruct FuncStruct(string s) => ReturnStruct;
		public static ReturnStruct FuncStruct() => ReturnStruct;

		public static ReturnClass FuncClass(ReturnClass s) => ReturnClass;
		public static ReturnClass FuncClass(ReturnStruct s) => ReturnClass;
		public static ReturnClass FuncClass(int i) => ReturnClass;
		public static ReturnClass FuncClass(string s) => ReturnClass;
		public static ReturnClass FuncClass() => ReturnClass;
	}

	[TestClass]
	public class DotNetFuncTest : DotNetTests
	{
		private ClassFunc classFunc = new ClassFunc();
		private StructFunc structFunc = new StructFunc();

		public static ReturnStruct ReturnStruct = new ReturnStruct() { Value = 1 };
		public static ReturnClass ReturnClass = new ReturnClass() { Value = 1 };

		public void Action(ReturnStruct s) { }
		public void Action(ReturnClass s) { }
		public void Action(int i) { }
		public void Action(string s) { }
		public void Action() { }

		public int FuncInt(ReturnStruct s) => 1;
		public int FuncInt(ReturnClass s) => 1;
		public int FuncInt(int i) => 1;
		public int FuncInt(string s) => 1;
		public int FuncInt() => 1;

		public ReturnStruct FuncStruct(ReturnStruct s) => ReturnStruct;
		public ReturnStruct FuncStruct(ReturnClass s) => ReturnStruct;
		public ReturnStruct FuncStruct(int i) => ReturnStruct;
		public ReturnStruct FuncStruct(string s) => ReturnStruct;
		public ReturnStruct FuncStruct() => ReturnStruct;

		public ReturnClass FuncClass(ReturnClass s) => ReturnClass;
		public ReturnClass FuncClass(ReturnStruct s) => ReturnClass;
		public ReturnClass FuncClass(int i) => ReturnClass;
		public ReturnClass FuncClass(string s) => ReturnClass;
		public ReturnClass FuncClass() => ReturnClass;

		#region Struct

		[TestMethod]
		public void StructActionTest()
			=> RunTest(nameof(StructActionTest), StructAction);

		public Stopwatch StructAction()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithStringParamTest()
			=> RunTest(nameof(StructActionWithStringParamTest), StructActionWithStringParam);

		public Stopwatch StructActionWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithIntParamTest()
			=> RunTest(nameof(StructActionWithIntParamTest), StructActionWithIntParam);

		public Stopwatch StructActionWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithConstStructParamTest()
			=> RunTest(nameof(StructActionWithConstStructParamTest), StructActionWithConstStructParam);

		public Stopwatch StructActionWithConstStructParam()
		{
			var s = new ReturnStruct() {Value = 1};
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithStructParamTest()
			=> RunTest(nameof(StructActionWithStructParamTest), StructActionWithStructParam);

		public Stopwatch StructActionWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithConstClassParamTest()
			=> RunTest(nameof(StructActionWithConstClassParamTest), StructActionWithConstClassParam);

		public Stopwatch StructActionWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructActionWithClassParamTest()
			=> RunTest(nameof(StructActionWithClassParamTest), StructActionWithClassParam);

		public Stopwatch StructActionWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.Action(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StructFuncIntTest()
			=> RunTest(nameof(StructFuncIntTest), StructFuncInt);

		public Stopwatch StructFuncInt()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithStringParamTest()
			=> RunTest(nameof(StructFuncIntWithStringParamTest), StructFuncIntWithStringParam);

		public Stopwatch StructFuncIntWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithIntParamTest()
			=> RunTest(nameof(StructFuncIntWithIntParamTest), StructFuncIntWithIntParam);

		public Stopwatch StructFuncIntWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithConstStructParamTest()
			=> RunTest(nameof(StructFuncIntWithConstStructParamTest), StructFuncIntWithConstStructParam);

		public Stopwatch StructFuncIntWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithStructParamTest()
			=> RunTest(nameof(StructFuncIntWithStructParamTest), StructFuncIntWithStructParam);

		public Stopwatch StructFuncIntWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithConstClassParamTest()
			=> RunTest(nameof(StructFuncIntWithConstClassParamTest), StructFuncIntWithConstClassParam);

		public Stopwatch StructFuncIntWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncIntWithClassParamTest()
			=> RunTest(nameof(StructFuncIntWithClassParamTest), StructFuncIntWithClassParam);

		public Stopwatch StructFuncIntWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncInt(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StructFuncStructTest()
			=> RunTest(nameof(StructFuncStructTest), StructFuncStruct);

		public Stopwatch StructFuncStruct()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithStringParamTest()
			=> RunTest(nameof(StructFuncStructWithStringParamTest), StructFuncStructWithStringParam);

		public Stopwatch StructFuncStructWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithIntParamTest()
			=> RunTest(nameof(StructFuncStructWithIntParamTest), StructFuncStructWithIntParam);

		public Stopwatch StructFuncStructWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithConstStructParamTest()
			=> RunTest(nameof(StructFuncStructWithConstStructParamTest), StructFuncStructWithConstStructParam);

		public Stopwatch StructFuncStructWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithStructParamTest()
			=> RunTest(nameof(StructFuncStructWithStructParamTest), StructFuncStructWithStructParam);

		public Stopwatch StructFuncStructWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithConstClassParamTest()
			=> RunTest(nameof(StructFuncStructWithConstClassParamTest), StructFuncStructWithConstClassParam);

		public Stopwatch StructFuncStructWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncStructWithClassParamTest()
			=> RunTest(nameof(StructFuncStructWithClassParamTest), StructFuncStructWithClassParam);

		public Stopwatch StructFuncStructWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncStruct(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StructFuncClassTest()
			=> RunTest(nameof(StructFuncClassTest), StructFuncClass);

		public Stopwatch StructFuncClass()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithStringParamTest()
			=> RunTest(nameof(StructFuncClassWithStringParamTest), StructFuncClassWithStringParam);

		public Stopwatch StructFuncClassWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithIntParamTest()
			=> RunTest(nameof(StructFuncClassWithIntParamTest), StructFuncClassWithIntParam);

		public Stopwatch StructFuncClassWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithConstStructParamTest()
			=> RunTest(nameof(StructFuncClassWithConstStructParamTest), StructFuncClassWithConstStructParam);

		public Stopwatch StructFuncClassWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithStructParamTest()
			=> RunTest(nameof(StructFuncClassWithStructParamTest), StructFuncClassWithStructParam);

		public Stopwatch StructFuncClassWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithConstClassParamTest()
			=> RunTest(nameof(StructFuncClassWithConstClassParamTest), StructFuncClassWithConstClassParam);

		public Stopwatch StructFuncClassWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StructFuncClassWithClassParamTest()
			=> RunTest(nameof(StructFuncClassWithClassParamTest), StructFuncClassWithClassParam);

		public Stopwatch StructFuncClassWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				structFunc.FuncClass(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}

		#endregion

		#region Class

		[TestMethod]
		public void ClassActionTest()
			=> RunTest(nameof(ClassActionTest), ClassAction);

		public Stopwatch ClassAction()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithStringParamTest()
			=> RunTest(nameof(ClassActionWithStringParamTest), ClassActionWithStringParam);

		public Stopwatch ClassActionWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithIntParamTest()
			=> RunTest(nameof(ClassActionWithIntParamTest), ClassActionWithIntParam);

		public Stopwatch ClassActionWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithConstStructParamTest()
			=> RunTest(nameof(ClassActionWithConstStructParamTest), ClassActionWithConstStructParam);

		public Stopwatch ClassActionWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithStructParamTest()
			=> RunTest(nameof(ClassActionWithStructParamTest), ClassActionWithStructParam);

		public Stopwatch ClassActionWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithConstClassParamTest()
			=> RunTest(nameof(ClassActionWithConstClassParamTest), ClassActionWithConstClassParam);

		public Stopwatch ClassActionWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassActionWithClassParamTest()
			=> RunTest(nameof(ClassActionWithClassParamTest), ClassActionWithClassParam);

		public Stopwatch ClassActionWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.Action(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ClassFuncIntTest()
			=> RunTest(nameof(ClassFuncIntTest), ClassFuncInt);

		public Stopwatch ClassFuncInt()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithStringParamTest()
			=> RunTest(nameof(ClassFuncIntWithStringParamTest), ClassFuncIntWithStringParam);

		public Stopwatch ClassFuncIntWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithIntParamTest()
			=> RunTest(nameof(ClassFuncIntWithIntParamTest), ClassFuncIntWithIntParam);

		public Stopwatch ClassFuncIntWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithConstStructParamTest()
			=> RunTest(nameof(ClassFuncIntWithConstStructParamTest), ClassFuncIntWithConstStructParam);

		public Stopwatch ClassFuncIntWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithStructParamTest()
			=> RunTest(nameof(ClassFuncIntWithStructParamTest), ClassFuncIntWithStructParam);

		public Stopwatch ClassFuncIntWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithConstClassParamTest()
			=> RunTest(nameof(ClassFuncIntWithConstClassParamTest), ClassFuncIntWithConstClassParam);

		public Stopwatch ClassFuncIntWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncIntWithClassParamTest()
			=> RunTest(nameof(ClassFuncIntWithClassParamTest), ClassFuncIntWithClassParam);

		public Stopwatch ClassFuncIntWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncInt(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ClassFuncStructTest()
			=> RunTest(nameof(ClassFuncStructTest), ClassFuncStruct);

		public Stopwatch ClassFuncStruct()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithStringParamTest()
			=> RunTest(nameof(ClassFuncStructWithStringParamTest), ClassFuncStructWithStringParam);

		public Stopwatch ClassFuncStructWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithIntParamTest()
			=> RunTest(nameof(ClassFuncStructWithIntParamTest), ClassFuncStructWithIntParam);

		public Stopwatch ClassFuncStructWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithConstStructParamTest()
			=> RunTest(nameof(ClassFuncStructWithConstStructParamTest), ClassFuncStructWithConstStructParam);

		public Stopwatch ClassFuncStructWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithStructParamTest()
			=> RunTest(nameof(ClassFuncStructWithStructParamTest), ClassFuncStructWithStructParam);

		public Stopwatch ClassFuncStructWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithConstClassParamTest()
			=> RunTest(nameof(ClassFuncStructWithConstClassParamTest), ClassFuncStructWithConstClassParam);

		public Stopwatch ClassFuncStructWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncStructWithClassParamTest()
			=> RunTest(nameof(ClassFuncStructWithClassParamTest), ClassFuncStructWithClassParam);

		public Stopwatch ClassFuncStructWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncStruct(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ClassFuncClassTest()
			=> RunTest(nameof(ClassFuncClassTest), ClassFuncClass);

		public Stopwatch ClassFuncClass()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithStringParamTest()
			=> RunTest(nameof(ClassFuncClassWithStringParamTest), ClassFuncClassWithStringParam);

		public Stopwatch ClassFuncClassWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithIntParamTest()
			=> RunTest(nameof(ClassFuncClassWithIntParamTest), ClassFuncClassWithIntParam);

		public Stopwatch ClassFuncClassWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithConstStructParamTest()
			=> RunTest(nameof(ClassFuncClassWithConstStructParamTest), ClassFuncClassWithConstStructParam);

		public Stopwatch ClassFuncClassWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithStructParamTest()
			=> RunTest(nameof(ClassFuncClassWithStructParamTest), ClassFuncClassWithStructParam);

		public Stopwatch ClassFuncClassWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithConstClassParamTest()
			=> RunTest(nameof(ClassFuncClassWithConstClassParamTest), ClassFuncClassWithConstClassParam);

		public Stopwatch ClassFuncClassWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ClassFuncClassWithClassParamTest()
			=> RunTest(nameof(ClassFuncClassWithClassParamTest), ClassFuncClassWithClassParam);

		public Stopwatch ClassFuncClassWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				classFunc.FuncClass(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}

		#endregion

		#region This

		[TestMethod]
		public void ThisActionTest()
			=> RunTest(nameof(ThisActionTest), ThisAction);

		public Stopwatch ThisAction()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithStringParamTest()
			=> RunTest(nameof(ThisActionWithStringParamTest), ThisActionWithStringParam);

		public Stopwatch ThisActionWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithIntParamTest()
			=> RunTest(nameof(ThisActionWithIntParamTest), ThisActionWithIntParam);

		public Stopwatch ThisActionWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithConstStructParamTest()
			=> RunTest(nameof(ThisActionWithConstStructParamTest), ThisActionWithConstStructParam);

		public Stopwatch ThisActionWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithStructParamTest()
			=> RunTest(nameof(ThisActionWithStructParamTest), ThisActionWithStructParam);

		public Stopwatch ThisActionWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithConstClassParamTest()
			=> RunTest(nameof(ThisActionWithConstClassParamTest), ThisActionWithConstClassParam);

		public Stopwatch ThisActionWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisActionWithClassParamTest()
			=> RunTest(nameof(ThisActionWithClassParamTest), ThisActionWithClassParam);

		public Stopwatch ThisActionWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				Action(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ThisFuncIntTest()
			=> RunTest(nameof(ThisFuncIntTest), ThisFuncInt);

		public Stopwatch ThisFuncInt()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithStringParamTest()
			=> RunTest(nameof(ThisFuncIntWithStringParamTest), ThisFuncIntWithStringParam);

		public Stopwatch ThisFuncIntWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithIntParamTest()
			=> RunTest(nameof(ThisFuncIntWithIntParamTest), ThisFuncIntWithIntParam);

		public Stopwatch ThisFuncIntWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithConstStructParamTest()
			=> RunTest(nameof(ThisFuncIntWithConstStructParamTest), ThisFuncIntWithConstStructParam);

		public Stopwatch ThisFuncIntWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithStructParamTest()
			=> RunTest(nameof(ThisFuncIntWithStructParamTest), ThisFuncIntWithStructParam);

		public Stopwatch ThisFuncIntWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithConstClassParamTest()
			=> RunTest(nameof(ThisFuncIntWithConstClassParamTest), ThisFuncIntWithConstClassParam);

		public Stopwatch ThisFuncIntWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncIntWithClassParamTest()
			=> RunTest(nameof(ThisFuncIntWithClassParamTest), ThisFuncIntWithClassParam);

		public Stopwatch ThisFuncIntWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncInt(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ThisFuncStructTest()
			=> RunTest(nameof(ThisFuncStructTest), ThisFuncStruct);

		public Stopwatch ThisFuncStruct()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithStringParamTest()
			=> RunTest(nameof(ThisFuncStructWithStringParamTest), ThisFuncStructWithStringParam);

		public Stopwatch ThisFuncStructWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithIntParamTest()
			=> RunTest(nameof(ThisFuncStructWithIntParamTest), ThisFuncStructWithIntParam);

		public Stopwatch ThisFuncStructWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithConstStructParamTest()
			=> RunTest(nameof(ThisFuncStructWithConstStructParamTest), ThisFuncStructWithConstStructParam);

		public Stopwatch ThisFuncStructWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithStructParamTest()
			=> RunTest(nameof(ThisFuncStructWithStructParamTest), ThisFuncStructWithStructParam);

		public Stopwatch ThisFuncStructWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithConstClassParamTest()
			=> RunTest(nameof(ThisFuncStructWithConstClassParamTest), ThisFuncStructWithConstClassParam);

		public Stopwatch ThisFuncStructWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncStructWithClassParamTest()
			=> RunTest(nameof(ThisFuncStructWithClassParamTest), ThisFuncStructWithClassParam);

		public Stopwatch ThisFuncStructWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncStruct(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void ThisFuncClassTest()
			=> RunTest(nameof(ThisFuncClassTest), ThisFuncClass);

		public Stopwatch ThisFuncClass()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithStringParamTest()
			=> RunTest(nameof(ThisFuncClassWithStringParamTest), ThisFuncClassWithStringParam);

		public Stopwatch ThisFuncClassWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithIntParamTest()
			=> RunTest(nameof(ThisFuncClassWithIntParamTest), ThisFuncClassWithIntParam);

		public Stopwatch ThisFuncClassWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithConstStructParamTest()
			=> RunTest(nameof(ThisFuncClassWithConstStructParamTest), ThisFuncClassWithConstStructParam);

		public Stopwatch ThisFuncClassWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithStructParamTest()
			=> RunTest(nameof(ThisFuncClassWithStructParamTest), ThisFuncClassWithStructParam);

		public Stopwatch ThisFuncClassWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithConstClassParamTest()
			=> RunTest(nameof(ThisFuncClassWithConstClassParamTest), ThisFuncClassWithConstClassParam);

		public Stopwatch ThisFuncClassWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void ThisFuncClassWithClassParamTest()
			=> RunTest(nameof(ThisFuncClassWithClassParamTest), ThisFuncClassWithClassParam);

		public Stopwatch ThisFuncClassWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				FuncClass(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}

		#endregion

		#region Static

		[TestMethod]
		public void StaticActionTest()
			=> RunTest(nameof(StaticActionTest), StaticAction);

		public Stopwatch StaticAction()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithStringParamTest()
			=> RunTest(nameof(StaticActionWithStringParamTest), StaticActionWithStringParam);

		public Stopwatch StaticActionWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithIntParamTest()
			=> RunTest(nameof(StaticActionWithIntParamTest), StaticActionWithIntParam);

		public Stopwatch StaticActionWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithConstStructParamTest()
			=> RunTest(nameof(StaticActionWithConstStructParamTest), StaticActionWithConstStructParam);

		public Stopwatch StaticActionWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithStructParamTest()
			=> RunTest(nameof(StaticActionWithStructParamTest), StaticActionWithStructParam);

		public Stopwatch StaticActionWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithConstClassParamTest()
			=> RunTest(nameof(StaticActionWithConstClassParamTest), StaticActionWithConstClassParam);

		public Stopwatch StaticActionWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticActionWithClassParamTest()
			=> RunTest(nameof(StaticActionWithClassParamTest), StaticActionWithClassParam);

		public Stopwatch StaticActionWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.Action(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StaticFuncIntTest()
			=> RunTest(nameof(StaticFuncIntTest), StaticFuncInt);

		public Stopwatch StaticFuncInt()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithStringParamTest()
			=> RunTest(nameof(StaticFuncIntWithStringParamTest), StaticFuncIntWithStringParam);

		public Stopwatch StaticFuncIntWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithIntParamTest()
			=> RunTest(nameof(StaticFuncIntWithIntParamTest), StaticFuncIntWithIntParam);

		public Stopwatch StaticFuncIntWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithConstStructParamTest()
			=> RunTest(nameof(StaticFuncIntWithConstStructParamTest), StaticFuncIntWithConstStructParam);

		public Stopwatch StaticFuncIntWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithStructParamTest()
			=> RunTest(nameof(StaticFuncIntWithStructParamTest), StaticFuncIntWithStructParam);

		public Stopwatch StaticFuncIntWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithConstClassParamTest()
			=> RunTest(nameof(StaticFuncIntWithConstClassParamTest), StaticFuncIntWithConstClassParam);

		public Stopwatch StaticFuncIntWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncIntWithClassParamTest()
			=> RunTest(nameof(StaticFuncIntWithClassParamTest), StaticFuncIntWithClassParam);

		public Stopwatch StaticFuncIntWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncInt(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StaticFuncStructTest()
			=> RunTest(nameof(StaticFuncStructTest), StaticFuncStruct);

		public Stopwatch StaticFuncStruct()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithStringParamTest()
			=> RunTest(nameof(StaticFuncStructWithStringParamTest), StaticFuncStructWithStringParam);

		public Stopwatch StaticFuncStructWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithIntParamTest()
			=> RunTest(nameof(StaticFuncStructWithIntParamTest), StaticFuncStructWithIntParam);

		public Stopwatch StaticFuncStructWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithConstStructParamTest()
			=> RunTest(nameof(StaticFuncStructWithConstStructParamTest), StaticFuncStructWithConstStructParam);

		public Stopwatch StaticFuncStructWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithStructParamTest()
			=> RunTest(nameof(StaticFuncStructWithStructParamTest), StaticFuncStructWithStructParam);

		public Stopwatch StaticFuncStructWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithConstClassParamTest()
			=> RunTest(nameof(StaticFuncStructWithConstClassParamTest), StaticFuncStructWithConstClassParam);

		public Stopwatch StaticFuncStructWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncStructWithClassParamTest()
			=> RunTest(nameof(StaticFuncStructWithClassParamTest), StaticFuncStructWithClassParam);

		public Stopwatch StaticFuncStructWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncStruct(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void StaticFuncClassTest()
			=> RunTest(nameof(StaticFuncClassTest), StaticFuncClass);

		public Stopwatch StaticFuncClass()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass();
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithStringParamTest()
			=> RunTest(nameof(StaticFuncClassWithStringParamTest), StaticFuncClassWithStringParam);

		public Stopwatch StaticFuncClassWithStringParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass("123");
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithIntParamTest()
			=> RunTest(nameof(StaticFuncClassWithIntParamTest), StaticFuncClassWithIntParam);

		public Stopwatch StaticFuncClassWithIntParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithConstStructParamTest()
			=> RunTest(nameof(StaticFuncClassWithConstStructParamTest), StaticFuncClassWithConstStructParam);

		public Stopwatch StaticFuncClassWithConstStructParam()
		{
			var s = new ReturnStruct() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithStructParamTest()
			=> RunTest(nameof(StaticFuncClassWithStructParamTest), StaticFuncClassWithStructParam);

		public Stopwatch StaticFuncClassWithStructParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass(new ReturnStruct() { Value = 1 });
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithConstClassParamTest()
			=> RunTest(nameof(StaticFuncClassWithConstClassParamTest), StaticFuncClassWithConstClassParam);

		public Stopwatch StaticFuncClassWithConstClassParam()
		{
			var s = new ReturnClass() { Value = 1 };
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass(s);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void StaticFuncClassWithClassParamTest()
			=> RunTest(nameof(StaticFuncClassWithClassParamTest), StaticFuncClassWithClassParam);

		public Stopwatch StaticFuncClassWithClassParam()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				StaticClass.FuncClass(new ReturnClass() { Value = 1 });
			sw.Stop();
			return sw;
		}

		#endregion
	}
}