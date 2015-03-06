using System;
using System.Runtime.Caching;
using Bolt.Cache.Dto;

namespace Bolt.Cache.InMemory
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly int _priority;

        public InMemoryCacheProvider(int priority)
        {
            _priority = priority;
        }

        public int Priority { get { return _priority; } }

        public virtual string Name { get { return "InMemoryCacheProvider"; } }
        
        public bool Exists(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        public CacheResult<T> Get<T>(string key)
        {
            var result = MemoryCache.Default.Get(key);

            return result == null 
                    ? CacheResult<T>.Empty 
                    : new CacheResult<T>(true, (T)result);
        }

        public void Set<T>(string key, T value, int durationInSeconds)
        {
            if(value == null) return;

            MemoryCache.Default.Add(key, value, DateTimeOffset.UtcNow.AddSeconds(durationInSeconds));
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}
