using System;

namespace Core.ElasticSearch.Exceptions
{
	public class QueryException : Exception
	{
		public string DebugInformation { get; }
		public QueryException(string info)
		{
			DebugInformation = info;
		}
	}
}