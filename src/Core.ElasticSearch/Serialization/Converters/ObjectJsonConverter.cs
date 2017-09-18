using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	/// <inheritdoc />
	/// <summary>
	/// Конвертер для вложенных документов (структур)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class ObjectJsonConverter<T> : JsonConverter where T : struct
	{
		private readonly PropertyInfo[] _properties;
		private readonly ActivatorData<T> _creator;

		public ObjectJsonConverter()
		{
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_creator = typeof(T).GetConstructors().FirstOrDefault().NotNullOrDefault(ObjectActivator.GetActivator<T>);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in _properties)
			{
				var v = property.GetValue(value);
				if (v.IsNull(property.PropertyType))
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (_creator.Parameters != null)
			{
				var parameters = new Dictionary<string, object>(
					_creator.Parameters.Select(x => KeyValuePair.Create<string, object>(x.Key, x.Value.GetDefaultValue())));
				while (reader.Read() && reader.TokenType != JsonToken.EndObject)
				{
					if (reader.TokenType != JsonToken.PropertyName || reader.Value == null)
						throw new Exception("TokenType != JsonToken.PropertyName");

					reader.Value.As<string>()
						.IfNotNull(
							x => x.If(p => reader.Read() && parameters.ContainsKey(p),
								p => parameters[p] = serializer.Deserialize(reader, _creator.Parameters.First(z => z.Key.Equals(p)).Value)));
				}
				return _creator.Creator(_creator.Parameters.Select(x => parameters[x.Key]).ToArray());
			}
			var jsonObject = JObject.Load(reader);
			var target = existingValue ?? new T();
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}