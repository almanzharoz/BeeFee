using System;

namespace Core.ElasticSearch
{
	[AttributeUsage(AttributeTargets.Constructor)]
	public class DeserializeConstructorAttribute : Attribute
	{
	}
}