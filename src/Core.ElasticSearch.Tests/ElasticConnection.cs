using System;
using Core.ElasticSearch;

namespace Core.ElasticSearch.Tests
{
	public class ElasticConnection : BaseElasticConnection
	{
		public ElasticConnection() : base(new Uri("http://localhost:9200"))
		{
		}

		public readonly string FirstIndex = "first_test_index";
		public readonly string SecondIndex = "second_test_index";
	}
}