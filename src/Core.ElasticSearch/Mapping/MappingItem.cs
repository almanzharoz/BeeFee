using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch.Mapping
{
	internal interface IMappingItem
	{
		string IndexName { get; }
		string TypeName { get; }
		IPutMappingResponse Map(ElasticClient client, Func<Type, IMappingItem> getMappingItem);
		MappingsDescriptor Map(MappingsDescriptor descriptor, Func<Type, IMappingItem> getMappingItem);
	}

	internal class MappingItem<T, TSettings> : IMappingItem 
		where T : class, IModel
		where TSettings : BaseElasticConnection
	{
		private readonly IEnumerable<string> _fields;
		public MappingItem(TSettings settings, Func<TSettings, string> indexName)
		{
			_fields = typeof(T).GetFieldsNames();
			TypeName = typeof(T).GetTypeInfo().GetCustomAttribute<ElasticsearchTypeAttribute>()?.Name ?? typeof(T).Name.ToLower();
			IndexName = indexName(settings);
		}

		public IEnumerable<string> CheckFields(IEnumerable<string> fields)
			=> fields.Except(_fields).ToImmutableArray(); // TODO: Сделать проверку типов

		public string IndexName { get; }
		public string TypeName { get; }

		public IPutMappingResponse Map(ElasticClient client, Func<Type, IMappingItem> getMappingItem)
			=> client.Map<T>(x => x.AutoMap()
				.Index(IndexName)
				.Fluent(g => GetParentType()
					.IfNotNull(z => g.Parent(getMappingItem(z).TypeName))));

		public MappingsDescriptor Map(MappingsDescriptor descriptor, Func<Type, IMappingItem> getMappingItem)
			=> descriptor.Map<T>(x => x.AutoMap()
				.Fluent(g => GetParentType()
					.IfNotNull(z => g.Parent(getMappingItem(z).TypeName))));

		private Type GetParentType()
			=> typeof(T).GetTypeInfo()
				.GetInterfaces()
				.FirstOrDefault(y => y.Name.IndexOf("IWithParent") == 0)
				?.GenericTypeArguments.FirstOrDefault();

		// TODO: Проверять тип парента, чтобы не было возможности указать парентом другой тип (IWithParent<T> where T : IModel)
	}
}