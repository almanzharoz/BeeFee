using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Serialization;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	internal interface IRequestContainer
	{
		T GetOrAdd<T>(T entity) where T : class, IProjection;
		//T AddOrUpdate<T>(T entity) where T : class, IProjection;
		//T Get<T>(string key) where T : class, IProjection, IJoinProjection, new();
		IEntity Get(string key);
		IEnumerable<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)> PopEntitiesForLoad();
	}
	/// <summary>
	/// Контейнер загруженных объектов и объектов, готовых для загрузки.
	/// </summary>
	/// <typeparam name="TSettings"></typeparam>
	internal class RequestContainer<TSettings> : IRequestContainer 
		where TSettings : BaseElasticConnection
	{
		private readonly ConcurrentDictionary<string, IList<KeyValuePair<IEntity, bool>>> _cache =
			new ConcurrentDictionary<string, IList<KeyValuePair<IEntity, bool>>>();

		private ConcurrentBag<IEntity> _loadBag = new ConcurrentBag<IEntity>();

		private object _locker = new object();
		private readonly ElasticMapping<TSettings> _mapping;

		public RequestContainer(ElasticMapping<TSettings> mapping)
		{
			_mapping = mapping;
			//Debug.WriteLine("Create Container "+this.GetHashCode());
		}

		//TODO: Возможно, нужно добавить GetOrAdd<T, TParent>(id, parent)
		public T GetOrAdd<T>(T entity) where T : class, IProjection
		{
			var type = typeof(T);
			return (T) _cache.AddOrUpdate(entity.Id,
					x =>
					{
						lock (_locker)
							_loadBag.Add(entity);
						var list = new List<KeyValuePair<IEntity, bool>>();
						list.Add(new KeyValuePair<IEntity, bool>(entity, true));
						return list;
					},
					(k, e) =>
					{
						if (e.All(x => x.Key.GetType() != type))
						{
							lock (_locker)
								_loadBag.Add(entity);
							e.Add(new KeyValuePair<IEntity, bool>(entity, true));
						}
						return e;
					})
				.First(x => x.Key.GetType() == type)
				.Key;
		}

		//public T AddOrUpdate<T>(T entity) where T : class, IProjection
		//{
		//	var type = typeof(T);
		//	return (T) _cache.AddOrUpdate(entity.Id, key =>
		//		{
		//			var list = new List<KeyValuePair<IEntity, bool>>();
		//			list.Add(new KeyValuePair<IEntity, bool>(entity, false));
		//			return list;

		//		}, (key, list) =>
		//		{
		//			if (list.All(x => x.Key.GetType() != type))
		//			{
		//				list.Add(new KeyValuePair<IEntity, bool>(entity, false));
		//			}
		//			else throw new Exception($"Second load type {type.Name} with Id: {entity.Id}");
		//			return list;

		//		})
		//		.First(x => x.Key.GetType() == type)
		//		.Key;
		//}

		//public T Get<T>(string key) where T : class, IProjection, IJoinProjection, new()
		//{
		//	var type = typeof(T);
		//	if(!_cache.TryGetValue(key, out var result))
		//		throw new KeyNotFoundException();
		//	return (T)result.First(x => x.Key.GetType() == type).Key;
		//}

		public IEntity Get(string key)
		{
			// Нужная проекция определяется из порядка запроса
			if (_cache.TryGetValue(key, out IList<KeyValuePair<IEntity, bool>> result))
			{
				var res = result.FirstOrDefault(x => x.Value);
				if (res.NotNull())
				{
					result[result.IndexOf(res)] = new KeyValuePair<IEntity, bool>(res.Key, false);
					return res.Key;
				}
			}
			return null;
		}

		public IEnumerable<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)> PopEntitiesForLoad()
		{
			IEntity[] items;
			lock (_locker)
			{
				items = _loadBag.ToArray();
				_loadBag = new ConcurrentBag<IEntity>();
			}
			// группировать по MappingItem
			var result =
				new List<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)>();
			foreach (var item in items.GroupBy(x => x.GetType())
				.Select(x => (projection: _mapping.GetProjectionItem(x.Key), ids: x.Select(y => y.Id).ToArray())))
			{
				result.Add((
					index: item.projection.MappingItem.IndexName,
					types: new[] {item.projection.MappingItem.TypeName},
					fields: item.projection.Fields,
					ids: item.ids));
			}
			return result;
		}

		public void ClearCache()
		{
			_cache.Clear();
			_loadBag = new ConcurrentBag<IEntity>();
		}
	}
}
