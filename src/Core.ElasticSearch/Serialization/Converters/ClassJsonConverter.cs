using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
	{
		public bool Equals(PropertyInfo x, PropertyInfo y)
		{
			return x.Name.Equals(y.Name);
		}

		public int GetHashCode(PropertyInfo obj)
		{
			return 0;
		}
	}

	internal class ClassJsonConverter<T> : JsonConverter where T : class, IProjection
	{
		private readonly IProjectionItem _projectionItem;
		private readonly ActivatorData<T> _creator;
		private readonly Dictionary<string, Action<T, object>> _setters = new Dictionary<string, Action<T, object>>();

		public ClassJsonConverter(IProjectionItem projectionItem)
		{
			_projectionItem = projectionItem;
			_creator = typeof(T).GetConstructors().FirstOrDefault().NotNullOrDefault(ObjectActivator.GetActivator<T>);
			_setters = _projectionItem.Properties.Where(x => x.SetMethod != null).ToDictionary(x => x.Name.ToLowerInvariant(), ObjectActivator.GetSetter<T>);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in _projectionItem.Properties)
			{
				var v = property.GetValue(value);
				if (v.IsNull(property.PropertyType))
					continue;
				var o = JToken.FromObject(new InnerValue(v), serializer);
				writer.WritePropertyName(property.Name.ToLower());
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (_creator.Parameters == null)
				throw new Exception($"Not create {typeof(T).Name}");

			if (reader.TokenType == JsonToken.String)
				return ((CoreElasticContractResolver)serializer.ContractResolver).Container.GetOrAdd<T>(_creator.Creator(reader.Value as string));

			var values = new Dictionary<string, object>();
			var parameters = new Dictionary<string, object>(
				_creator.Parameters.Select(x => KeyValuePair.Create<string, object>(x.Key, x.Value.GetDefaultValue())));
			long version = 0;
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType != JsonToken.PropertyName || reader.Value == null)
					throw new Exception("TokenType != JsonToken.PropertyName");

				reader.Value.As<string>()
					.IfNotNull(
						x => x.If(p => reader.Read(),
							p => p.If(z => parameters.ContainsKey(z),
								y =>
								parameters[y] = serializer.Deserialize(reader, _creator.Parameters.First(z => z.Key.Equals(y)).Value),
								y =>
								{
									if (y == "_type") return null;
									if (y == "version")
									{
										version = (long) reader.Value;
										return null;
									}
									values.Add(y,
										serializer.Deserialize(reader, _projectionItem.Properties.First(z => z.Name.ToLower() == y).PropertyType));
									return null;
								})
						));
			}
			var target =  _creator.Creator(_creator.Parameters.Select(x => parameters[x.Key]).ToArray());
			
			foreach (var value in values)
				_setters[value.Key](target, value.Value);

			if (target is IWithVersion && version > 0)
				(target as BaseEntityWithVersion).Version = (int)version;

			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new System.NotImplementedException();
		}
	}
}