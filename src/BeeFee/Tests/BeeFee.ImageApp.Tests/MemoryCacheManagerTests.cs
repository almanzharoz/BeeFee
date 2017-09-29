using System;
using System.Collections.Generic;
using System.Text;
using BeeFee.ImageApp.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp.Tests
{
    [TestClass]
    public class MemoryCacheManagerTests
    {
        private MemoryCacheManager _cacheManager;

        [TestInitialize]
        public void Setup()
        {
            _cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
        }

        [TestMethod]
        public void SetAndGetObjectFromCache()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);

            _cacheManager.Get<int>("some_key_1").Equals(3);
        }

        [TestMethod]
        public void ValidateWhetherObjectIsCached()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);
            _cacheManager.Set("some_key_2", 4, int.MaxValue);

            _cacheManager.IsSet("some_key_1").Equals(true);
            _cacheManager.IsSet("some_key_3").Equals(false);
        }

        [TestMethod]
        public void GetOrSetObjectFromCache()
        {
            _cacheManager.Get("some_key", () => int.MaxValue).Equals(int.MaxValue);

            _cacheManager.Get<int>("some_key").Equals(int.MaxValue);
        }

        [TestMethod]
        public void RemoveByPattern()
        {
            _cacheManager.Set("some_key_1_1", 3, int.MaxValue);
            _cacheManager.Set("some_key_2_1", 4, int.MaxValue);
            _cacheManager.Set("some_key_2_2", 5, int.MaxValue);

            _cacheManager.RemoveByPattern("some_key_2");

            _cacheManager.IsSet("some_key_1_1").Equals(true);
            _cacheManager.IsSet("some_key_2_1").Equals(false);
            _cacheManager.IsSet("some_key_2_2").Equals(false);
        }

        [TestMethod]
        public void ClearCache()
        {
            _cacheManager.Set("some_key_1", 3, int.MaxValue);

            _cacheManager.Clear();

            _cacheManager.IsSet("some_key_1").Equals(false);
        }
    }
}
