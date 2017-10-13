using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp7Func
{
    public class CatchCollection<TArg, TResult>
	{
		private readonly string _type;
		private readonly TArg _arg;
		private readonly Func<TArg, TResult> _func;
		private readonly Dictionary<Type, IExceptionHandler<TArg>> _handlers = new Dictionary<Type, IExceptionHandler<TArg>>();

		internal CatchCollection(string type, TArg arg, Func<TArg, TResult> func)
		{
			_type = type;
			_arg = arg;
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
			_handlers.Add(typeof(T), new ExceptionHandler<T,TArg>(handler));
			return this;
		}

		public TResult Catch()
		{
			try
			{
				return _func(_arg);
			}
			catch (Exception e)
			{
				if (_handlers.ContainsKey(e.GetType()))
					throw new HandledException(_type, _handlers[e.GetType()].Use(e, _arg), e);
				throw;
			}
		}
    }

	public class CatchCollection<TArg>
	{
		private readonly string _type;
		private readonly TArg _arg;
		private readonly Action<TArg> _func;
		private readonly Dictionary<Type, IExceptionHandler<TArg>> _handlers = new Dictionary<Type, IExceptionHandler<TArg>>();

		internal CatchCollection(string type, TArg arg, Action<TArg> func)
		{
			_type = type;
			_arg = arg;
			_func = func;
		}

		internal CatchCollection(TArg arg, Action<TArg> func)
		{
			_type = null;
			_arg = arg;
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
			_handlers.Add(typeof(T), new ExceptionHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg> Catch<T>(Action<T, TArg> handler) where T : Exception
		{
			_handlers.Add(typeof(T), new ExceptionSimpleHandler<T, TArg>(handler));
			return this;
		}

		public CatchCollection<TArg> Catch<T>() where T : Exception
		{
			_handlers.Add(typeof(T), new ExceptionHandler<T, TArg>(DefaultHandler));
			return this;
		}

		private string DefaultHandler<T>(T exception, TArg arg) where T : Exception
			=> exception.Message;

		/// <summary>
		/// Без обработки ошибок - прокидывание дальше
		/// </summary>
		public void Throw()
		{
			try
			{
				_func(_arg);
			}
			catch (Exception e)
			{
				if (_handlers.ContainsKey(e.GetType()))
					throw new HandledException(_type, _handlers[e.GetType()].Use(e, _arg), e);
				throw;
			}
		}

		public string Catch()
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
					sb.AppendLine(_handlers.ContainsKey(e.GetType()) ? _handlers[e.GetType()].Use(e, _arg) : exception.Message);
				return sb.ToString();
			}
			catch (Exception e)
			{
				if (_handlers.ContainsKey(e.GetType()))
					return _handlers[e.GetType()].Use(e, _arg);
				throw;
			}
		}

		//public void Catch(Action<TArg, string> errorHandler)
		//{
		//	try
		//	{
		//		_func(_arg);
		//	}
		//	catch (Exception e)
		//	{
		//		if (_handlers.ContainsKey(e.GetType()))
		//			errorHandler(_arg, _handlers[e.GetType()].Use(e, _arg));
		//		else throw;
		//	}
		//}

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
