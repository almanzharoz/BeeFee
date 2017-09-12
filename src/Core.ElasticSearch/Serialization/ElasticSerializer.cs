using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	internal interface IElasticSerializer
	{
		Stopwatch _sw { get; }
		Stopwatch _sw2 { get; }
		CoreElasticContractResolver ContractResolver { get; }
	}
	internal class ElasticSerializer<TSettings> : IElasticsearchSerializer, IElasticSerializer
		where TSettings : BaseElasticConnection
	{
		private readonly ElasticMapping<TSettings> _mapping;
		private static readonly InnerValueJsonConverter _innerValueJsonConverter = new InnerValueJsonConverter();

		public ElasticSerializer(IConnectionSettingsValues settings, ElasticMapping<TSettings> mapping,
			RequestContainer<TSettings> container)
		{
			this.Settings = settings;
			_mapping = mapping;
			ContractResolver = new CoreElasticContractResolver(settings, this.ContractConverters, container);
			ContractConverters.Add(GetJsonConverter);

			var indented = new JsonSerializerSettings()
			{
				Formatting = Formatting.None,
				ContractResolver = this.ContractResolver,
				DefaultValueHandling = DefaultValueHandling.Include,
				NullValueHandling = NullValueHandling.Ignore
			};
			this._indentedSerializer = JsonSerializer.Create(indented);
		}

		private JsonConverter GetJsonConverter(Type x)
		{
			if (x == typeof(InnerValue))
				return _innerValueJsonConverter;
			if (x == typeof(IProjection))
				return new BaseClassJsonConverter<TSettings>();
			if (typeof(IProjection).IsAssignableFrom(x))
				return _mapping.GetJsonConverter(x);
			if (_mapping.TryGetResponseJsonConverter(x, out JsonConverter result))
				return result;
			return null;
		}

		protected IList<Func<Type, JsonConverter>> ContractConverters { get; } = new List<Func<Type, JsonConverter>>();

		private static readonly Encoding ExpectedEncoding = new UTF8Encoding(false);

		private JsonSerializer _indentedSerializer;

		protected IConnectionSettingsValues Settings { get; }

		/// <summary>
		/// Resolves JsonContracts for types
		/// </summary>
		public CoreElasticContractResolver ContractResolver { get; }

		/// <summary>
		/// The size of the buffer to use when writing the serialized request
		/// to the request stream
		/// </summary>
		// Performance tests as part of https://github.com/elastic/elasticsearch-net/issues/1899 indicate this 
		// to be a good compromise buffer size for performance throughput and bytes allocated.
		protected int BufferSize => 1024;

		protected static readonly ConcurrentDictionary<string, IPropertyMapping> Properties = new ConcurrentDictionary<string, IPropertyMapping>();

		public IPropertyMapping CreatePropertyMapping(MemberInfo memberInfo)
			=> Properties.GetOrAdd($"{memberInfo.DeclaringType?.FullName}.{memberInfo.Name}", key => PropertyMappingFromAttributes(memberInfo));

		private static IPropertyMapping PropertyMappingFromAttributes(MemberInfo memberInfo)
		{
			var jsonProperty = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(true);
			var ignoreProperty = memberInfo.GetCustomAttribute<JsonIgnoreAttribute>(true);
			if (jsonProperty == null && ignoreProperty == null) return null;
			return new PropertyMapping { Name = jsonProperty?.PropertyName, Ignore = ignoreProperty != null };
		}

		public void Serialize(object data, Stream writableStream, SerializationFormatting formatting = SerializationFormatting.Indented)
		{
			using (var writer = new StreamWriter(writableStream, ExpectedEncoding, BufferSize, leaveOpen: true))
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				//var t = _sw2.ElapsedMilliseconds;
				//_sw2.Start();
				_indentedSerializer.Serialize(jsonWriter, data);
				//_sw2.Stop();
				//Console.WriteLine($"Serialize<{data.GetType().FullName}> "+(_sw2.ElapsedMilliseconds-t));
				writer.Flush();
				jsonWriter.Flush();
			}
		}

		public Stopwatch _sw => new Stopwatch();
		public Stopwatch _sw2 => new Stopwatch();

		public T Deserialize<T>(Stream stream)
		{
			if (stream == null) return default(T);
			using (var streamReader = new StreamReader(stream))
			using (var jsonTextReader = new JsonTextReader(streamReader))
			{
				//var t = _sw.ElapsedMilliseconds;
				//_sw.Start();
				var result = _indentedSerializer.Deserialize<T>(jsonTextReader);
				//_sw.Stop();
				//Console.WriteLine($"Deserialize<{typeof(T).FullName}> "+(_sw.ElapsedMilliseconds-t));
				return result;
			}
		}

		public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
			=> Task.FromResult(Deserialize<T>(stream));
	}

}