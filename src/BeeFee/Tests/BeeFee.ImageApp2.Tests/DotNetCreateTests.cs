using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp2.Tests
{
	public struct StructWithReadonlyProperty
	{
		public int Value { get; }

		public StructWithReadonlyProperty(int value)
		{
			Value = value;
		}
	}

	public struct StructWithReadonlyField
	{
		public readonly int Value;

		public StructWithReadonlyField(int value)
		{
			Value = value;
		}
	}

	public struct StructWithPrivateField
	{
		private int Value;

		public StructWithPrivateField(int value)
		{
			Value = value;
		}
	}

	public struct StructWithReadonlyPrivateField
	{
		private readonly int Value;

		public StructWithReadonlyPrivateField(int value)
		{
			Value = value;
		}
	}

	public struct StructWithField
	{
		public int Value;

		public StructWithField(int value)
		{
			Value = value;
		}
	}

	public struct StructWithProperty
	{
		public int Value { get; set; }

		public StructWithProperty(int value)
		{
			Value = value;
		}
	}


	public class ClassWithReadonlyProperty
	{
		public int Value { get; }

		public ClassWithReadonlyProperty(int value)
		{
			Value = value;
		}
	}

	public class ClassWithReadonlyField
	{
		public readonly int Value;

		public ClassWithReadonlyField(int value)
		{
			Value = value;
		}
	}

	public class ClassWithReadonlyPrivateField
	{
		private readonly int Value;

		public ClassWithReadonlyPrivateField(int value)
		{
			Value = value;
		}
	}

	public class ClassWithPrivateField
	{
		private readonly int Value;

		public ClassWithPrivateField(int value)
		{
			Value = value;
		}
	}

	public class ClassWithField
	{
		public int Value;

		public ClassWithField() { }
		public ClassWithField(int value)
		{
			Value = value;
		}
	}

	public class ClassWithProperty
	{
		public int Value { get; set; }

		public ClassWithProperty() { }
		public ClassWithProperty(int value)
		{
			Value = value;
		}
	}

	public abstract class DotNetTests
	{
		protected readonly int countOfInnerIterations = 10000000;

		protected void RunTest(string name, Func<Stopwatch> test, int count = 100)
		{
			Console.WriteLine($"{name} with {countOfInnerIterations} iter. and {count} times:");
			var totalswEmpty = new Stopwatch[count];
			var totalsw = new Stopwatch[count];
			for (var i = 0; i < count; i++)
			{
				var emptySw = new Stopwatch();
				emptySw.Start();
				for (var j = 0; j < countOfInnerIterations; j++) ;
				emptySw.Stop();
				totalswEmpty[i] = emptySw;

				var sw = test();
				totalsw[i] = sw;

				//Console.WriteLine($"{i}: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");
			}
			Console.WriteLine($"Average Empty for: {totalswEmpty.Average(x => x.ElapsedMilliseconds)} ms, {totalswEmpty.Average(x => x.ElapsedTicks)} ticks");
			Console.WriteLine($"Average: {totalsw.Average(x => x.ElapsedMilliseconds)} ms, {totalsw.Average(x => x.ElapsedTicks)} ticks");
			Console.WriteLine($"Clear: {totalsw.Average(x => x.ElapsedMilliseconds) - totalswEmpty.Average(x => x.ElapsedMilliseconds)} ms, {totalsw.Average(x => x.ElapsedTicks) - totalswEmpty.Average(x => x.ElapsedTicks)} ticks");
		}
	}

	[TestClass]
	public class DotNetCreateTests : DotNetTests
	{

		[TestMethod]
		public void CreateStructWithReadonlyPropertyAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithReadonlyPropertyAndParamConstructorTest), CreateStructWithReadonlyPropertyAndParamConstructor);

		public Stopwatch CreateStructWithReadonlyPropertyAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithReadonlyProperty(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithReadonlyFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithReadonlyFieldAndParamConstructorTest), CreateStructWithReadonlyFieldAndParamConstructor);

		public Stopwatch CreateStructWithReadonlyFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithReadonlyField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithReadonlyPrivateFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithReadonlyPrivateFieldAndParamConstructorTest), CreateStructWithReadonlyPrivateFieldAndParamConstructor);

		public Stopwatch CreateStructWithReadonlyPrivateFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithReadonlyPrivateField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithPropertyAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithPropertyAndParamConstructorTest), CreateStructWithPropertyAndParamConstructor);

		public Stopwatch CreateStructWithPropertyAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithProperty(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithFieldAndParamConstructorTest), CreateStructWithFieldAndParamConstructor);

		public Stopwatch CreateStructWithFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithPrivateFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateStructWithPrivateFieldAndParamConstructorTest), CreateStructWithPrivateFieldAndParamConstructor);

		public Stopwatch CreateStructWithPrivateFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithPrivateField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithPropertyAndDefaultConstructorTest()
			=> RunTest(nameof(CreateStructWithPropertyAndDefaultConstructorTest), CreateStructWithPropertyAndDefaultConstructor);

		public Stopwatch CreateStructWithPropertyAndDefaultConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithProperty() {Value = i};
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateStructWithFieldAndDefaultConstructorTest()
			=> RunTest(nameof(CreateStructWithFieldAndDefaultConstructorTest), CreateStructWithFieldAndDefaultConstructor);

		public Stopwatch CreateStructWithFieldAndDefaultConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new StructWithField() {Value = i};
			sw.Stop();
			return sw;
		}


		[TestMethod]
		public void CreateClassWithReadonlyPropertyAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithReadonlyPropertyAndParamConstructorTest), CreateClassWithReadonlyPropertyAndParamConstructor);

		public Stopwatch CreateClassWithReadonlyPropertyAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithReadonlyProperty(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithReadonlyFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithReadonlyFieldAndParamConstructorTest), CreateClassWithReadonlyFieldAndParamConstructor);

		public Stopwatch CreateClassWithReadonlyFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithReadonlyField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithReadonlyPrivateFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithReadonlyPrivateFieldAndParamConstructorTest), CreateClassWithReadonlyPrivateFieldAndParamConstructor);

		public Stopwatch CreateClassWithReadonlyPrivateFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithReadonlyPrivateField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithPrivateFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithPrivateFieldAndParamConstructorTest), CreateClassWithPrivateFieldAndParamConstructor);

		public Stopwatch CreateClassWithPrivateFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithPrivateField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithPropertyAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithPropertyAndParamConstructorTest), CreateClassWithPropertyAndParamConstructor);

		public Stopwatch CreateClassWithPropertyAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithProperty(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithFieldAndParamConstructorTest()
			=> RunTest(nameof(CreateClassWithFieldAndParamConstructorTest), CreateClassWithFieldAndParamConstructor);

		public Stopwatch CreateClassWithFieldAndParamConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithField(i);
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithPropertyAndDefaultConstructorTest()
			=> RunTest(nameof(CreateClassWithPropertyAndDefaultConstructorTest), CreateClassWithPropertyAndDefaultConstructor);

		public Stopwatch CreateClassWithPropertyAndDefaultConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithProperty() { Value = i };
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithFieldAndDefaultConstructorTest()
			=> RunTest(nameof(CreateClassWithFieldAndDefaultConstructorTest), CreateClassWithFieldAndDefaultConstructor);

		public Stopwatch CreateClassWithFieldAndDefaultConstructor()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithField() { Value = i };
			sw.Stop();
			return sw;
		}

		[TestMethod]
		public void CreateClassWithFieldAndDefaultConstructorAndFieldSetTest()
			=> RunTest(nameof(CreateClassWithFieldAndDefaultConstructorAndFieldSetTest), CreateClassWithFieldAndDefaultConstructorAndFieldSet);

		public Stopwatch CreateClassWithFieldAndDefaultConstructorAndFieldSet()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < countOfInnerIterations; i++)
				new ClassWithField().Value = i;
			sw.Stop();
			return sw;
		}

		private delegate void DelegateChangeStruct(ref StructWithField s);

		[TestMethod]
		public void RefStructTest()
		{
			var s = new StructWithField(5);
			ChangeStruct(ref s, ChangeStructAction);
			Console.WriteLine(s.Value);
		}

		private void ChangeStructAction(ref StructWithField s)
		{
			s.Value = 10;
		}

		private void ChangeStruct(ref StructWithField s, DelegateChangeStruct Method)
		{
			Method(ref s);
		}
	}
}
