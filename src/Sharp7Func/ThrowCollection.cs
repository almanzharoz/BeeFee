using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SharpFuncExt
{
	public class ThrowCollection : IEnumerable<Exception>
	{
		private readonly List<Exception> _exceptions = new List<Exception>();

		internal ThrowCollection Add<T>(T exception) where T : Exception
			=> this.Fluent(x => _exceptions.Add(exception));

		public void Throw()
		{
			if (_exceptions.Count > 0)
				throw new AggregateException(_exceptions);
		}

		public IEnumerator<Exception> GetEnumerator() => _exceptions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static class ThrowCollectionExtensions
	{
		public static T HasNotNullArg<T>(this T arg, ThrowCollection collection, string argName) => arg.ThrowIfNull(collection, () => new ArgumentNullException(argName));
		public static T HasNotNullArg<T, TValue>(this T arg, ThrowCollection collection, Expression<Func<T, TValue>> expression, string argName) => arg.ThrowIfNull(collection, expression, () => new ArgumentNullException(argName));
		public static T HasNotNullArg<T, TValue1, TValue2>(this T arg, ThrowCollection collection, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2, string argName)
			=> arg.ThrowIfNull(collection, expression1, expression2, () => new ArgumentNullException(argName));

		public static T ThrowIfNull<T, TException>(this T arg, ThrowCollection collection, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull())
				collection.Add(func());
			return arg;
		}

		public static T ThrowIfNull<T, TException>(this T arg, ThrowCollection collection) where TException : Exception, new()
		{
			if (arg.IsNull())
				collection.Add(new TException());
			return arg;
		}

		public static T ThrowIfNull<T, TValue, TException>(this T arg, ThrowCollection collection, Expression<Func<T, TValue>> expression, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull(expression))
				collection.Add(func());
			return arg;
		}

		public static T ThrowIfNull<T, TValue1, TValue2, TException>(this T arg, ThrowCollection collection, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull(expression1, expression2))
				collection.Add(func());
			return arg;
		}

		public static T ThrowIfNull<T, TValue1, TValue2, TValue3, TException>(this T arg, ThrowCollection collection, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2, Expression<Func<T, TValue3>> expression3, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull(expression1, expression2, expression3))
				collection.Add(func());
			return arg;
		}
	}
}