using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Exceptions;
using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Serialization;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
    public abstract partial class BaseService<TConnection>
        where TConnection : BaseElasticConnection
    {
        protected readonly ILogger _logger;
        protected readonly ElasticClient _client;
        private readonly ElasticClient<TConnection> _topclient;
		private readonly RequestContainer<TConnection> _container;
        private readonly TConnection _settings;
        private readonly ElasticMapping<TConnection> _mapping;

        protected BaseService(ILoggerFactory loggerFactory, TConnection settings, ElasticScopeFactory<TConnection> factory)
        {
            _container = factory.Container;
            _settings = settings;
            _mapping = factory.Mapping;
	        _topclient = factory.Client;
            _client = factory.Client.Client;
            _logger = loggerFactory.CreateLogger<BaseService<TConnection>>();
        }

        protected async Task LoadAsync()
        {
            // TODO: Проблема: могут запрашиваться поля не входящие в проекцию, т.к. одинаково имя поля из другой проекции
            var entitiesToLoad = _container.PopEntitiesForLoad();
            do
            {
                foreach (var item in entitiesToLoad)
                {
#if DEBUG
                    _logger.LogDebug($"Loading data: {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
                    _logger.LogDebug((await _client.SearchAsync<IProjection>(x => x
                        .Index(item.index)
                        .Type(Types.Type(item.types))
                        .Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
                        .Query(q => q.Ids(id => id.Values(item.ids))))).DebugInformation);
#else
				await _client.SearchAsync<IProjection>(x => x
					.Index(item.index)
					.Type(Types.Type(item.types))
					.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
					.Query(q => q.Ids(id => id.Values(item.ids))));
#endif
                }
            } while ((entitiesToLoad = _container.PopEntitiesForLoad()).Any());
        }

        protected void Load()
        {
            var sw = new Stopwatch();
            sw.Start();
            // TODO: Проблема: могут запрашиваться поля не входящие в проекцию, т.к. одинаково имя из другой проекции
            var entitiesToLoad = _container.PopEntitiesForLoad();
            do
            {
                foreach (var item in entitiesToLoad)
                {
#if DEBUG
                    _logger.LogDebug($"Loading data: {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
                    _logger.LogDebug((_client.Search<IProjection>(x => x
                        .Index(item.index)
                        .Type(Types.Type(item.types))
                        .Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
                        .Query(q => q.Ids(id => id.Values(item.ids))))).DebugInformation);
#else
				_client.Search<IProjection>(x => x
					.Index(item.index)
					.Type(Types.Type(item.types))
					.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
					.Query(q => q.Ids(id => id.Values(item.ids))));
#endif
                }
            } while ((entitiesToLoad = _container.PopEntitiesForLoad()).Any());
            sw.Stop();
            Console.WriteLine("Load: " + sw.ElapsedMilliseconds);
        }

	    protected void ClearCache() => _container.ClearCache();

		#region Try
		protected TResult Try<TResponse, TResult>(Func<ElasticClient, TResponse> func, Func<TResponse, TResult> result, EventId eventId, string operationText = null)
            where TResponse : IResponse
        {
            try
            {
				Stopwatch sw = new Stopwatch();
	            return result(_client.Stopwatch(sw, x => func(_client))
		            .Fluent(x =>
			            Console.WriteLine("Try: " + sw.ElapsedMilliseconds
			                              + ", Deserialize: " + ((IElasticSerializer) _client.Serializer)._sw.ElapsedMilliseconds
			                              + ", Serialize: " + ((IElasticSerializer) _client.Serializer)._sw2.ElapsedMilliseconds
			                              + ", Deserialize: " + ((IElasticSerializer)_client.Serializer).ContractResolver.sw2.ElapsedMilliseconds
			                              + ", Serialize: " + ((IElasticSerializer)_client.Serializer).ContractResolver.sw1.ElapsedMilliseconds
										  ))
		            .LogDebug(_logger, operationText ?? eventId.Name)
		            .LogError(_logger, operationText ?? eventId.Name));
            }
            catch (Exception e)
            {
                _logger.LogCritical(eventId, e, operationText ?? eventId.Name);
                throw;
            }
        }

        protected async Task<TResult> TryAsync<TResponse, TResult>(Func<ElasticClient, Task<TResponse>> func, Func<TResponse, TResult> result, EventId eventId, string operationText = null)
            where TResponse : IResponse
        {
            try
            {
				Stopwatch sw = new Stopwatch();
				sw.Start();
				var f = await func(_client);
				sw.Stop();
				return result(f
	                .Fluent(x =>
		                Console.WriteLine("Try: " + sw.ElapsedMilliseconds
		                                  + ", Deserialize: " + ((IElasticSerializer)_client.Serializer)._sw.ElapsedMilliseconds
		                                  + ", Serialize: " + ((IElasticSerializer)_client.Serializer)._sw2.ElapsedMilliseconds
		                                  + ", Deserialize: " + ((IElasticSerializer)_client.Serializer).ContractResolver.sw2.ElapsedMilliseconds
		                                  + ", Serialize: " + ((IElasticSerializer)_client.Serializer).ContractResolver.sw1.ElapsedMilliseconds
		                ))
					.LogDebug(_logger, operationText ?? eventId.Name)
                    .LogError(_logger, operationText ?? eventId.Name));
            }
            catch (Exception e)
            {
                _logger.LogCritical(eventId, e, operationText ?? eventId.Name);
                throw;
            }
        }

        protected async Task<TResult> TryAsync<TResponse, TResult>(Func<ElasticClient, Task<TResponse>> func, Func<TResponse, Task<TResult>> result, EventId eventId, string operationText = null)
            where TResponse : IResponse
        {
            try
            {
                return await result((await func(_client))
                    .LogDebug(_logger, operationText ?? eventId.Name)
                    .LogError(_logger, operationText ?? eventId.Name));
            }
            catch (Exception e)
            {
                _logger.LogCritical(eventId, e, operationText ?? eventId.Name);
                throw;
            }
        }
        #endregion Try
    }

    public static class RepositoryLoggingEvents
    {
        public static EventId ES_SEARCH = new EventId(1000, "Search");
        public static EventId ES_GET = new EventId(1001, "Get");
        public static EventId ES_INSERT = new EventId(1002, "Insert");
        public static EventId ES_UPDATE = new EventId(1003, "Update");
        public static EventId ES_UPDATEBYQUERY = new EventId(1004, "UpdateByQuery");
        public static EventId ES_REMOVE = new EventId(1005, "Remove");
        public static EventId ES_REMOVEBYQUERY = new EventId(1006, "RemoveByQuery");
        public static EventId ES_COUNT = new EventId(1007, "Count");

    }

    internal static class ElasticExtensions
    {
        public static T LogError<T>(this T arg, ILogger logger, string text) where T : IResponse
            => arg.IfNot(x => x.IsValid, x => logger
                .Fluent(z => z.LogError($"{text}: {x.ServerError.ToString()}\r\n{x.DebugInformation}"))
                .Switch(l => x.ServerError.Error.Type == "version_conflict_engine_exception", l => throw new VersionException())
                .SwitchDefault(z => throw new QueryException(x.DebugInformation)));

        public static T LogDebug<T>(this T arg, ILogger logger, string text = null) where T : IResponse
        {
#if DEBUG
            if (arg.IsValid)
                logger.LogDebug(!String.IsNullOrWhiteSpace(text) ? $"{text}: {arg.DebugInformation}" : arg.DebugInformation);
#endif
            return arg;
        }

    }
}
