using System;

namespace Core.ElasticSearch
{
	/// <summary>
	/// Базовый класс настройки подключения к БД
	/// </summary>
	public abstract class BaseElasticConnection
	{
		public Uri Url { get; }

		protected BaseElasticConnection(Uri url)
		{
			Url = url;
		}
	}
}