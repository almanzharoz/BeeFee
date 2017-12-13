using System;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public interface IException { }

	public interface IBoolResult
	{
		bool Success { get; }
	}

	public struct BoolResult : IBoolResult
	{
		public bool Success { get; }
		public IException Exception { get; }

		public BoolResult(bool success)
		{
			Success = success;
			Exception = null;
		}

		public BoolResult(IException exception)
		{
			Exception = exception;
			Success = false;
		}

		public static bool operator &(BoolResult arg1, IBoolResult arg2) => arg1.Success & arg2.Success;
		public static bool operator |(BoolResult arg1, IBoolResult arg2) => arg1.Success | arg2.Success;
		public static bool operator ^(BoolResult arg1, IBoolResult arg2) => arg1.Success ^ arg2.Success;

		public static bool operator &(BoolResult arg1, bool arg2) => arg1.Success & arg2;
		public static bool operator |(BoolResult arg1, bool arg2) => arg1.Success | arg2;
		public static bool operator ^(BoolResult arg1, bool arg2) => arg1.Success ^ arg2;

		public static bool operator true(BoolResult arg) => arg.Success;
		public static bool operator false(BoolResult arg) => !arg.Success;
	}

	public struct BoolResult<TResult>:IBoolResult
	{
		public bool Success { get; }
		public TResult Result { get; }
		public IException Exception { get; }

		public BoolResult(bool success, TResult result)
		{
			Success = success;
			Result = result;
			Exception = null;
		}

		public BoolResult(IException exception)
		{
			Exception = exception;
			Success = false;
			Result = default(TResult);
		}

		public static bool operator &(BoolResult<TResult> arg1, IBoolResult arg2) => arg1.Success & arg2.Success;
		public static bool operator |(BoolResult<TResult> arg1, IBoolResult arg2) => arg1.Success | arg2.Success;
		public static bool operator ^(BoolResult<TResult> arg1, IBoolResult arg2) => arg1.Success ^ arg2.Success;

		public static bool operator &(BoolResult<TResult> arg1, bool arg2) => arg1.Success & arg2;
		public static bool operator |(BoolResult<TResult> arg1, bool arg2) => arg1.Success | arg2;
		public static bool operator ^(BoolResult<TResult> arg1, bool arg2) => arg1.Success ^ arg2;

		public static bool operator true(BoolResult<TResult> arg) => arg.Success;
		public static bool operator false(BoolResult<TResult> arg) => !arg.Success;
	}

	public static class SharpLogic
	{
		public static bool Try<TArg>(this TArg arg, Func<TArg, bool> func, Action<TArg, Exception> catchAction)
			=> arg.Try(func, (a, e) =>
			{
				catchAction(a, e);
				return false;
			});

		public static bool Rollback<TArg>(this TArg arg, Func<TArg, bool> func, Func<TArg, bool> rollbackFunc)
			=> func(arg) || rollbackFunc(arg) && false;
		//{
		//	var f = func(arg);
		//	if (!f)
		//		rollbackFunc(arg);
		//	return f;
		//}

		public static bool Rollback<TArg, TResult>(this TArg arg, Func<TArg, BoolResult<TResult>> func,
			Func<TArg, TResult, bool> rollbackFunc)
		{
			var r = func(arg);
			return r.Success || rollbackFunc(arg, r.Result) && false;
		}

		public static async Task<bool> Rollback<TArg>(this TArg arg, Func<TArg, Task<bool>> func,
			Func<TArg, Task<bool>> rollbackFunc)
			=> await func(arg) || await rollbackFunc(arg) && false;

		public static async Task<bool> Rollback<TArg>(this TArg arg, Func<TArg, Task<bool>> func,
			Func<TArg, bool> rollbackFunc)
			=> await func(arg) || rollbackFunc(arg) && false;

		public static async Task<bool> Rollback<TArg, TResult>(this TArg arg, Func<TArg, Task<BoolResult<TResult>>> func,
			Func<TArg, TResult, Task<bool>> rollbackFunc)
		{
			var r = await func(arg);
			return r.Success || await rollbackFunc(arg, r.Result) && false;
		}

		/// <summary>
		/// Вместо или.
		/// </summary>
		/// <example>(func1() && func2()).Always(Loging) && func3()</example>
		/// <param name="arg"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static bool Always(this bool arg, Action action)
		{
			action();
			return arg;
		}

		public static BoolResult<TResult> Always<TResult>(this BoolResult<TResult> arg, Action<BoolResult<TResult>> action)
		{
			action(arg);
			return arg;
		}

		public static BoolResult IfThrow(this BoolResult arg, Action<BoolResult> action)
		{
			if (arg.Exception != null)
				action(arg);
			return arg;
		}

		public static BoolResult<TResult> IfThrow<TResult>(this BoolResult<TResult> arg, Action<BoolResult<TResult>> action)
		{
			if (arg.Exception != null)
				action(arg);
			return arg;
		}

	}
}