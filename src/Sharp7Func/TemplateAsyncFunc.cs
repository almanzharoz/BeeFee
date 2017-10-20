using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class TemplateAsyncFunc
	{
		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, bool> check, Func<T, Task<TResult>> ifTrue,
				Func<T, Task<TResult>> ifFalse)
			=> await (check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, bool> check,
			Func<T, Task<TResult>> ifTrue,
			Func<T, Task<TResult>> ifFalse)
		{
			var a = await arg;
			return await (check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, Task<bool>> check,
				Func<T, Task<TResult>> ifTrue,
				Func<T, Task<TResult>> ifFalse)
			=> await (await check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, Task<bool>> check,
			Func<T, Task<TResult>> ifTrue,
			Func<T, Task<TResult>> ifFalse)
		{
			var a = await arg;
			return await (await check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, bool> check,
			Func<T, TResult> ifTrue,
			Func<T, TResult> ifFalse)
		{
			var a = await arg;
			return (check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, Task<bool>> check,
				Func<T, TResult> ifTrue,
				Func<T, TResult> ifFalse)
			=> (await check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, Task<bool>> check,
			Func<T, TResult> ifTrue,
			Func<T, TResult> ifFalse)
		{
			var a = await arg;
			return (await check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<T> If<T, TResult>(this T arg, Func<T, bool> check, Func<T, Task<TResult>> ifTrue)
		{
			if (check(arg))
				await ifTrue(arg);
			return arg;
		}

		public static async Task<T> IfAsync<T>(this T arg, bool check, Func<Task> ifTrue)
		{
			if (check)
				await ifTrue();
			return arg;
		}


		public static async Task<TResult> IfNotNull<T, TResult>(this Task<T> arg, Func<T, TResult> ifTrue, Func<TResult> ifNull)
		{
			var a = await arg;
			return (!a.IsNull() ? ifTrue(a) : ifNull());
		}

		public static async Task<TResult> IfNotNullAsync<T, TResult>(this Task<T> arg, Func<T, Task<TResult>> ifTrue, Func<Task<TResult>> ifNull)
		{
			var a = await arg;
			return await (a.IsNotNull() ? ifTrue(a) : ifNull());
		}

		public static async Task<TResult> IfNotNull<T, TResult, TResult2>(this Task<T> arg, Func<T, Task<TResult>> ifNotNull, Func<TResult2> ifNull)
			where TResult2 : TResult
		{
			var a = await arg;
			if (a.IsNotNull())
				return await ifNotNull(a);
			return (TResult)ifNull();
		}



		public static async Task<T> ThrowIf<T, TException>(this Task<T> arg, Func<T, bool> check, Func<T, TException> func) where TException : Exception
		{
			var a = await arg;
			if (check(a))
				throw func(a);
			return a;
		}

		public static Task<TResult> Try<T, TResult>(this T arg, Func<T, Task> func, Func<T, TResult> done,
			Func<T, Exception, TResult> @catch)
		{
			try
			{
				return func(arg)
					.ContinueWith(t => t.Exception != null ? @catch(arg, t.Exception) : done(arg));
			}
			catch (Exception e)
			{
				return Task.FromResult(@catch(arg, e));
			}
		}

		public static async Task<T> Stopwatch<T>(this Task<T> arg, Stopwatch sw, Func<T, T> func)
		{
			var a = await arg;
			sw.Start();
			func(a);
			sw.Stop();
			return a;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this Task<T> arg, Stopwatch sw, Func<T, TResult> func)
		{
			var a = await arg;
			sw.Start();
			var r = func(a);
			sw.Stop();
			return r;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this Task<T> arg, Stopwatch sw, Func<T, Task<TResult>> func)
		{
			var a = await arg;
			sw.Start();
			var r = await func(a);
			sw.Stop();
			return r;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this T arg, Stopwatch sw, Func<T, Task<TResult>> func)
		{
			sw.Start();
			var r = await func(arg);
			sw.Stop();
			return r;
		}

		//public static async Task<TResult> Using<T, TResult>(this T arg, Func<T, Task<TResult>> func) where T : IDisposable
		//{
		//	using (arg)
		//		return await func(arg);
		//}
		public static Task<TResult> Using<T, TResult>(this T arg, Func<T, Task<TResult>> func) where T : IDisposable
			=> func(arg).ContinueWith(x =>
			{
				arg.Dispose();
				return x.Result;
			});

		public static Task Using<T>(this T arg, Func<T, Task> func) where T : IDisposable
			=> func(arg).ContinueWith(x => arg.Dispose());

		public static async Task Using<T, TUsing>(this T arg, Func<T, TUsing> init, Func<T, TUsing, Task> func) where TUsing : IDisposable
		{
			Exception ex = null;
			TUsing u = init(arg);
			try
			{
				await func(arg, u);
			}
			catch (Exception e)
			{
				throw ex = e;
			}
			finally
			{
				try
				{
					u?.Dispose();
				}
				catch (Exception e)
				{
					throw ex != null ? new AggregateException(ex, e) : e;
				}
			}
		}
	}
}
