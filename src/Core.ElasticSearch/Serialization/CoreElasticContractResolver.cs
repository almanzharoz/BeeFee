using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	internal class CoreElasticContractResolver : ElasticContractResolver
	{
		public IRequestContainer Container { get; }
		public CoreElasticContractResolver(IConnectionSettingsValues connectionSettings, IList<Func<Type, JsonConverter>> contractConverters, IRequestContainer container) : base(connectionSettings, contractConverters)
		{
			Container = container;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var p = base.CreateProperty(member, memberSerialization);
			if (!p.Writable)
			{
				var property = member as PropertyInfo;
				if (property != null)
				{
					var hasPrivateSetter = property.GetSetMethod(true) != null;
					p.Writable = hasPrivateSetter;
				}
			}
			return p;
		}

		private static readonly ConcurrentDictionary<Type, JsonContract> Cache = new ConcurrentDictionary<Type, JsonContract>();
		public override JsonContract ResolveContract(Type type)
			=> Cache.GetOrAdd(type.HasNotNullArg(nameof(type)), CreateContract);

		public Stopwatch sw1 = new Stopwatch();
		public Stopwatch sw2 = new Stopwatch();

		protected override JsonContract CreateContract(Type objectType)
		{
			var result = base.CreateContract(objectType);
			if (typeof(ICollection).IsAssignableFrom(result.UnderlyingType))
				return result;
			if (!typeof(ICollection).IsAssignableFrom(result.UnderlyingType))
			{
				typeof(JsonContract).GetField("_onDeserializingCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
					.SetValue(result, new List<SerializationCallback>() {(o, context) => sw2.Start()});
				typeof(JsonContract).GetField("_onDeserializedCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
					.SetValue(result, new List<SerializationCallback>() {(o, context) => sw2.Stop()});
				typeof(JsonContract).GetField("_onSerializingCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
					.SetValue(result, new List<SerializationCallback>() {(o, context) => sw1.Start()});
				typeof(JsonContract).GetField("_onSerializedCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
					.SetValue(result, new List<SerializationCallback>() {(o, context) => sw1.Stop()});
			}
			return result;
		}
	}
}