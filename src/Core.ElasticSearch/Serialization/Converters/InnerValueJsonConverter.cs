﻿using System;
using System.Collections;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal struct InnerValue
	{
		public object Value { get; }

		public InnerValue(object value)
		{
			Value = value;
		}
	}

	internal class InnerValueJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (((InnerValue)value).Value is IEntity)
			{
				writer.WriteValue(((IEntity)((InnerValue)value).Value).Id);
				return;
			}
			//if (((InnerValue)value).Value.GetType().IsArray && typeof(IEntity).IsAssignableFrom(((InnerValue)value).Value.GetType().GetElementType()))
			//{
			//	writer.WriteStartArray();
			//	foreach(var v in ((InnerValue)value).Value as IEnumerable)
			//		writer.WriteValue(((IEntity)v).Id);
			//	writer.WriteEndArray();
			//	return;
			//}
			var o = JToken.FromObject(((InnerValue)value).Value, serializer);
			o.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}