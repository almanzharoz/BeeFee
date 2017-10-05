using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class EnumerableFunc
	{
		public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> arg)
		{
			return arg ?? new T[0];
		}
		public static TValue Add<TValue>(this TValue value, ref TValue[] list)
		{
			list = (list ?? new TValue[0]).Union(new[] { value }).ToArray();
			return value;
		}

		public static TValue Add<TValue>(this TValue value, IList<TValue> list)
		{
			list.Add(value);
			return value;
		}

		public static TValue Add<TValue>(this TValue value, ref IEnumerable<TValue> list)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Add(value);
			else
				list = (list ?? new TValue[0]).Union(new [] { value });
			return value;
		}
		public static IEnumerable<TValue> Add<TValue>(this IEnumerable<TValue> list, TValue value)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Add(value);
			else
				return (list ?? new TValue[0]).Union(new[] { value });
			return list;
		}

		public static T Add<T, TValue>(this T list, TValue value) where T : ICollection<TValue>
		{
			list.Add(value);
			return list;
		}
		public static T AddEach<T, TValue>(this T list, IEnumerable<TValue> values) where T : ICollection<TValue>
		{
			foreach(var value in values)
				list.Add(value);
			return list;
		}

		public static T[] Add<T>(this T[] list, T value)
			=> list.Union(new []{value}).ToArray();

		public static T[] AddEach<T>(this T[] list, IEnumerable<T> values)
			=> list.Union(values).ToArray();


		public static T TryAdd<T, TValue>(this T list, TValue value) where T : IProducerConsumerCollection<TValue>
		{
			list.TryAdd(value);
			return list;
		}
		public static TValue TryAdd<T, TValue>(this TValue value, T list) where T : IProducerConsumerCollection<TValue>
		{
			list.TryAdd(value);
			return value;
		}
		public static T Add<T, TValue, TKey>(this T list, TKey key, TValue value) where T : IDictionary<TKey, TValue>
		{
			list.Add(key, value);
			return list;
		}
		public static TValue Add<T, TValue, TKey>(this TValue value, T list, TKey key) where T : IDictionary<TKey, TValue>
		{
			list.Add(key, value);
			return value;
		}

		public static T AddEach<T, TKey, TValue>(this T arg, IDictionary<TKey, TValue> values) where T : IDictionary<TKey, TValue>
		{
			foreach (var value in values)
				arg.Add(value);
			return arg;
		}

		// может потребоваться много памяти на больших коллекциях
		public static T AddEach<T, TKey, TValue>(this T arg, IDictionary<TKey, TValue> values, Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> func) where T : IDictionary<TKey, TValue>
		{
			foreach (var value in values)
				arg.Add(func(value));
			return arg;
		}

		public static TValue Remove<TValue>(this TValue value, ref TValue[] list)
		{
			list = (list ?? new TValue[0]).Except(new[] { value }).ToArray();
			return value;
		}

		public static TValue Remove<TValue>(this TValue value, IList<TValue> list)
		{
			list.Remove(value);
			return value;
		}

		public static TValue Remove<TValue>(this TValue value, ref IEnumerable<TValue> list)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Remove(value);
			else
				list = (list ?? new TValue[0]).Except(new[] { value });
			return value;
		}
		public static IEnumerable<TValue> Remove<TValue>(this IEnumerable<TValue> list, TValue value)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Remove(value);
			else
				return (list ?? new TValue[0]).Except(new[] { value });
			return list;
		}

		public static TResult Folding<T, TResult>(this IEnumerable<T> arg, Func<T, TResult, TResult> func)
		{
			TResult result = default(TResult);
			foreach (var a in arg)
				result = func(a, result);
			return result;
		}

		public static IEnumerable<T> Unfolding<TArg, T>(this TArg arg, Func<TArg, int, bool> check, Func<TArg, int, T> func)
		{
			for (var i = 0; check(arg, i); i++)
				yield return func(arg, i);
		}

		public static IEnumerable<T> Unfolding<TArg, T>(this TArg arg, int from, int to, Func<TArg, int, T> func)
		{
			for (var i = from; i <= to; i++)
				yield return func(arg, i);
		}

		public static IEnumerable<T> Each<T, TResult>(this IEnumerable<T> arg, Func<T, TResult> func)
		{
			if (arg != null)
				foreach (var v in arg)
					func(v);
			return arg;
		}
		public static IEnumerable<T> Each<T>(this IEnumerable<T> arg, Action<T> func)
		{
			if (arg != null)
				foreach (var v in arg)
					func(v);
			return arg;
		}

		public static TArg Each<T, TArg>(this TArg arg, IEnumerable<T> items, Action<T> func)
		{
			if (items != null)
				foreach (var v in items)
					func(v);
			return arg;
		}
		public static TArg Each<T, TArg, TResult>(this TArg arg, IEnumerable<T> items, Func<T, TResult> func)
		{
			if (items != null)
				foreach (var v in items)
					func(v);
			return arg;
		}

		public static IEnumerable<T> EachThrowIf<T, TException>(this IEnumerable<T> arg, Func<T, bool> check, Func<T, TException> func) where TException : Exception
		{
			if (arg != null)
				foreach (var v in arg)
					if (check(v)) throw func(v);
			return arg;
		}

		public static T[] EachThrowIf<T, TException>(this T[] arg, Func<T, bool> check, Func<T, TException> func) where TException : Exception
		{
			if (arg != null)
				foreach (var v in arg)
					if (check(v)) throw func(v);
			return arg;
		}

		public static bool In<T>(this T arg, IEnumerable<T> array) => arg.IfNotNull(array.Contains, ()=>false);
		public static bool In<T>(this IEnumerable<T> array, T arg) => arg.IfNotNull(array.Contains, ()=>false);

		public static TResult IfIn<T, TResult, TArray>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func, Func<T, TArray, IEnumerable<TArray>, TResult> ifNot)
		{
			return array.IsNotNull() && array.Contains(element) ? func(arg, element, array) : ifNot(arg, element, array);
		}
		public static TResult IfIn<T, TResult, TArray>(this T arg, IEnumerable<TArray> array, Func<T, TArray, bool> check, Func<T, TArray, IEnumerable<TArray>, TResult> func, Func<T, IEnumerable<TArray>, TResult> ifNot)
		{
			if (array.IsNull())
				return ifNot(arg, array);
			bool hasNext;
			var enumerator = array.GetEnumerator();
			while ((hasNext = enumerator.MoveNext()) && !check(arg, enumerator.Current)) ;
			if (hasNext)
				return func(arg, enumerator.Current, array);
			return ifNot(arg, array);
		}
		public static TResult IfIn<TResult, TArray>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func, Func<TArray, IEnumerable<TArray>, TResult> ifNotContains)
		{
			return array.IsNotNull() && array.Contains(element) ? func(element, array) : ifNotContains(element, array);
		}
		public static TResult IfIn<TResult, TArray>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func, Func<TArray, IEnumerable<TArray>, TResult> ifNotContains)
		{
			return array.IsNotNull() && array.Contains(element) ? func(element, array) : ifNotContains(element, array);
		}

		public static T DefaultIfNull<T, TValue>(this T arg) where T : IEnumerable<TValue>
			=> arg == null ? (T)(IEnumerable<TValue>)new TValue[0] : arg;
		public static T[] DefaultIfNull<T>(this T[] arg) => arg ?? new T[0];

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : default(TValue);
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : defaultValue;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value
				: defaultValueProvider();
		}


		#region Fluent-version

		public static T IfAny<T, TArray>(this T arg, IEnumerable<TArray> array, Func<T, T> func)
		{
			if (array != null && array.Any())
				return func(arg);
			return arg;
		}
		public static T IfNotAny<T, TArray>(this T arg, IEnumerable<TArray> array, Func<T, T> func)
		{
			if (array == null || !array.Any())
				return func(arg);
			return arg;
		}

		public static T IfIn<T, TArray, TResult>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNotNull() && array.Contains(element))
				func(arg, element, array);
			return arg;
		}
		public static T IfNotIn<T, TArray, TResult>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(arg, element, array);
			return arg;
		}

		public static IEnumerable<TArray> IfIn<TArray, TResult>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNotNull() && array.Contains(element))
				func(element, array);
			return array;
		}
		public static IEnumerable<TArray> IfNotIn<TArray, TResult>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(element, array);
			return array;
		}

		public static TArray IfIn<TArray, TResult>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if(array.IsNotNull() && array.Contains(element))
				func(element, array);
			return element;
		}
		public static TArray IfNotIn<TArray, TResult>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(element, array);
			return element;
		}
		#endregion

		public static bool SeqEquals<T>(this IEnumerable<T> arg, IEnumerable<T> value)
		{
			if (arg == null && value == null || ReferenceEquals(arg, value))
				return true;
			if (arg == null && !value.Any() || value == null && !arg.Any())
				return true;
			if (arg == null || value == null)
				return false;
			var valueEnumenator = value.GetEnumerator();
			foreach (var a in arg)
				if (!valueEnumenator.MoveNext() || !a.Equals(valueEnumenator.Current))
					return false;
			return !valueEnumenator.MoveNext();
		}

		public static TypedCollection AddTyped<T>(this T value)
		{
			var result = new TypedCollection();
			result.Add(value);
			return result;
		}

	}
}

