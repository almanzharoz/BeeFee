using System;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class GetJsonConverter<T> : JsonConverter where T : class, IProjection
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			if (jsonObject["_source"] != null)
			{
				if (jsonObject["_id"] != null)
					jsonObject["_source"]["id"] = jsonObject["_id"];
				if (jsonObject["_version"] != null)
					jsonObject["_source"]["version"] = jsonObject["_version"];
				jsonObject["_source"]["_type"] = jsonObject["_type"];
				if (jsonObject["_parent"] != null)
					jsonObject["_source"]["parent"] = jsonObject["_parent"];
			}
			var result = new GetResponse<T>();
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, result);
			//if (result.Source is IWithVersion)
			//	((IWithVersion)result.Source).Version = (int)result.Version;
			//if (typeof(T).GetTypeInfo().GetInterfaces().Any(z => z.Name.IndexOf("IWithParent") == 0))
			//	typeof(T).GetTypeInfo().GetProperty("Parent").SetValue(result.Source, _entityContainer.Get(result.Parent));
			//result.Source.Id = result.Id;
			return result;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}