using System;
using Core.ElasticSearch;

namespace BeeFee.Model
{
	public class BeefeeElasticConnection : BaseElasticConnection
	{
		public BeefeeElasticConnection(Uri url) : base(url)
		{
		}

		public virtual string EventIndexName => "event_index";
		public virtual string UserIndexName => "user_index";
	}
}