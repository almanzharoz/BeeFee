using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class WrappingFunc
	{
		public static Func<T> FluentFunc<T>(this T arg, Expression<Func<T, Action>> expression) => () =>
		{
			expression.Compile()(arg)();
			return arg;
		};

		public static T Fluent<T>(this T arg, Action func)
		{
			func();
			return arg;
		}
		public static T Fluent<T>(this T arg, Action<T> func)
		{
			func(arg);
			return arg;
		}
		public static T Fluent<T, T2>(this T arg, Func<T, T2> func)
		{
			func(arg);
			return arg;
		}

		public static Func<TArg, T> FluentFunc<T, TArg>(this T arg, Expression<Func<T, Action<TArg>>> expression) => x =>
		{
			expression.Compile()(arg)(x);
			return arg;
		};

		public static Func<TResult> Func<T, TResult>(this T arg, Expression<Func<T, Func<TResult>>> expression) => expression.Compile()(arg);

		public static Func<TArg, TResult> Func<T, TArg, TResult>(this T arg, Expression<Func<T, Func<TArg, TResult>>> expression) => expression.Compile()(arg);

		public static Func<T, TResult> ToFunc<T, TResult>(this TResult result, Action<T> action) => arg =>
		{
			action(arg);
			return result;
		};
		public static Func<T, TResult> ToFunc<T, TResult>(this TResult result, Action<T, TResult> action) => arg =>
		{
			action(arg, result);
			return result;
		};
		public static Func<T, T> ToFunc<T>(this T result, Action<T> action) => arg =>
		{
			action(arg);
			return result;
		};

		public static Func<T, TResult> Next<T, TMid, TResult>(this Func<T, TMid> func, Func<TMid, TResult> next) => x => next(func(x));

		public static TResult UseIn<TArg, TResult>(this TArg arg, Func<TArg, TResult> func) => func(arg);
		public static TResult UseIn<TArg1, TArg2, TResult>(this (TArg1, TArg2) arg, Func<TArg1, TArg2, TResult> func) => func(arg.Item1, arg.Item2);
		public static TResult UseIn<TArg1, TArg2, TArg3, TResult>(this (TArg1, TArg2, TArg3) arg, Func<TArg1, TArg2, TArg3, TResult> func) => func(arg.Item1, arg.Item2, arg.Item3);
	}
}
