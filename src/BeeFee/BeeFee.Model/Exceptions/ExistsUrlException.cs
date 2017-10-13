using System;
using BeeFee.Model.Interfaces;

namespace BeeFee.Model.Exceptions
{
	public class ExistsUrlException<T> : Exception where T : IWithUrl
	{
		public ExistsUrlException(string url)
			:base($"Документ с url=\"{url}\" уже существует.")
		{
		}
	}
}