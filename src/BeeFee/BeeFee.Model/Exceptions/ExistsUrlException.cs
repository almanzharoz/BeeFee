using System;
using BeeFee.Model.Interfaces;

namespace BeeFee.Model.Exceptions
{
	public interface IExistsUrlException { }

	public class ExistsUrlException<T> : Exception, IExistsUrlException where T : IWithUrl
	{
		public ExistsUrlException(string url)
			:base($"Документ с url=\"{url}\" уже существует.")
		{
		}
	}
}