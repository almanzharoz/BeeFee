using System;
using System.Diagnostics;
using System.Linq;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
    internal class SearchJsonConverter<T> : JsonConverter where T : class, IProjection
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            foreach (var j in jsonObject["hits"]["hits"].AsEnumerable())
            {
                if (j["_id"] != null)
                    j["_source"]["id"] = j["_id"];
                if (j["_version"] != null)
                    j["_source"]["version"] = j["_version"];
                j["_source"]["_type"] = j["_type"];
                if (j["_parent"] != null)
                    j["_source"]["parent"] = j["_parent"];
            }
            if (jsonObject["suggest"] != null)
            {
                foreach (var cj in jsonObject["suggest"].Children())
                {
                    foreach (var sj in cj.Children())
                    {
                        foreach (var tj in sj)
                        {
                            if (tj["options"] != null)
                            {
                                foreach (var oj in tj["options"].AsEnumerable())
                                {
                                    if (oj["_id"] != null)
                                        oj["_source"]["id"] = oj["_id"];
                                    if (oj["_version"] != null)
                                        oj["_source"]["version"] = oj["_version"];
                                    oj["_source"]["_type"] = oj["_type"];
                                    if (oj["_parent"] != null)
                                        oj["_source"]["parent"] = oj["_parent"];
                                }
                            }
                        }
                    }
                }
            }
            var target = new SearchResponse<T>();
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