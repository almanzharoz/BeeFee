using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFuncExt
{
	public abstract class BaseCatchCollection<TArg>
	{
		protected readonly string _type;
		protected readonly TArg _arg;
		private readonly Dictionary<Type, IExceptionHandler<TArg>> _handlers = new Dictionary<Type, IExceptionHandler<TArg>>();

		protected BaseCatchCollection(string type, TArg arg)
		{
			_type = type;
			_arg = arg;
		}

		protected string DefaultHandler<T>(T exception, TArg arg) where T : Exception
			=> exception.Message;

		internal void AddHandler<T>(IExceptionHandler<TArg> handler) where T : Exception
			=> _handlers.Add(typeof(T), handler);

		protected HandledException TryUseHandler(Exception e)
		{
			if (_handlers.ContainsKey(e.GetType()))
				new HandledException(_type, _handlers[e.GetType()].Use(e, _arg), e);
			return null;
		}
	}


	public class CatchCollection<TArg, TResult> : BaseCatchCollection<TArg>
	{
		private readonly Func<TArg, TResult> _func;

		internal CatchCollection(string type, TArg arg, Func<TArg, TResult> func) : base(type, arg)
		{
			_func = func;
		}

		internal CatchCollection(TArg arg, Func<TArg, TResult> func) : base(null, arg)
		{
			_func = func;
		}

		/// <summary>
		/// Добавляет обработчик указанной ошибки
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		/// <returns></returns>
		public CatchCollection<TArg, TResult> Catch<T>(Func<T, TArg, string> handler) where T : Exception
		{
			AddHandler<T>(new ExceptionHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg, TResult> Catch<T>(Action<T, TArg> handler) where T : Exception
		{
			AddHandler<T>(new ExceptionSimpleHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg, TResult> Catch<T>() where T : Exception
		{
			AddHandler<T>(new ExceptionHandler<T, TArg>(DefaultHandler));
			return this;
		}

		public TResult Throw()
		{
			try
			{
				return _func(_arg);
			}
			catch (AggregateException e)
			{
				throw new AggregateException(e.InnerExceptions.Select(x => TryUseHandler(x) ?? x));
			}
			catch (Exception e)
			{
				TryUseHandler(e).ThrowIf(x => x != null, x => x);
				throw;
			}
		}

		public TResult Use()
		{
			try
			{
				return _func(_arg);
			}
			catch (AggregateException e)
			{
				throw new AggregateException(e.InnerExceptions.Select(TryUseHandler).Where(x => x != null));
			}
			catch (Exception e)
			{
				TryUseHandler(e).ThrowIf(x => x != null, x => x);
			}
			return default(TResult);
		}

		public TArg UseFluent()
		{
			try
			{
				_func(_arg);
			}
			catch (AggregateException e)
			{
				throw new AggregateException(e.InnerExceptions.Select(TryUseHandler).Where(x => x != null));
			}
			catch (Exception e)
			{
				TryUseHandler(e).ThrowIf(x => x != null, x => x);
			}
			return _arg;
		}

		public string UseMessage(out TResult result)
		{
			result = default(TResult);
			try
			{
				result = _func(_arg);
				return null;
			}
			catch (AggregateException e)
			{
				var sb = new StringBuilder();
				foreach (var exception in e.InnerExceptions)
					sb.AppendLine(TryUseHandler(exception)?.Message ?? exception.Message);
				return sb.ToString();
			}
			catch (Exception e)
			{
				return TryUseHandler(e)?.Message ?? e.Message;
			}
		}
	}

	public class CatchCollection<TArg> : BaseCatchCollection<TArg>
	{
		private readonly Action<TArg> _func;

		internal CatchCollection(string type, TArg arg, Action<TArg> func) : base(type, arg)
		{
			_func = func;
		}

		internal CatchCollection(TArg arg, Action<TArg> func) : base(null, arg)
		{
			_func = func;
		}

		/// <summary>
		/// Добавляет обработчик указанной ошибки
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		/// <returns></returns>
		public CatchCollection<TArg> Catch<T>(Func<T, TArg, string> handler) where T : Exception
		{
			AddHandler<T>(new ExceptionHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg> Catch<T>(Action<T, TArg> handler) where T : Exception
		{
			AddHandler<T>(new ExceptionSimpleHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg> Catch<T>() where T : Exception
		{
			AddHandler<T>(new ExceptionHandler<T, TArg>(DefaultHandler));
			return this;
		}


		/// <summary>
		/// Без обработки ошибок - прокидывание входящей ошибки дальше или преобразование в HandledException
		/// </summary>
		public void Throw()
		{
			try
			{
				_func(_arg);
			}
			catch (Exception e)
			{
				TryUseHandler(e).ThrowIf(x => x != null, x => x);
				throw;
			}
		}

		public string UseMessage()
		{
			try
			{
				_func(_arg);
				return null;
			}
			catch (AggregateException e)
			{
				var sb = new StringBuilder();
				foreach (var exception in e.InnerExceptions)
					sb.AppendLine(TryUseHandler(exception)?.Message ?? exception.Message);
				return sb.ToString();
			}
			catch (Exception e)
			{
				return TryUseHandler(e)?.Message ?? e.Message;
			}
		}
	}

	public class HandledException : Exception
	{
		public string Type { get; }

		public HandledException(string type, string message, Exception e) : base(message, e)
		{
			Type = type;
		}
	}

	internal interface IExceptionHandler<in TArg>
	{
		string Use(Exception ex, TArg arg);
	}

	internal class ExceptionHandler<T, TArg> : IExceptionHandler<TArg> where T : Exception
	{
		private readonly Func<T, TArg, string> _handlerAction;

		public ExceptionHandler(Func<T, TArg, string> handlerAction)
		{
			_handlerAction = handlerAction;
		}

		public string Use(Exception ex, TArg arg) => _handlerAction((T)ex, arg);
	}

	internal class ExceptionSimpleHandler<T, TArg> : IExceptionHandler<TArg> where T : Exception
	{
		private readonly Action<T, TArg> _handlerAction;

		public ExceptionSimpleHandler(Action<T, TArg> handlerAction)
		{
			_handlerAction = handlerAction;
		}

		public string Use(Exception ex, TArg arg)
		{
			_handlerAction((T) ex, arg);
			return ex.Message;
		}
	}

}
