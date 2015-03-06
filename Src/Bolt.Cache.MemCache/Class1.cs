using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt.Cache.Dto;

namespace Bolt.Cache.MemCache
{
    public class MemCacheProvider : ICacheProvider
    {
        private readonly int _priority;

        public MemCacheProvider(int priority)
        {
            _priority = priority;
        }

        public int Priority { get { return _priority; } }
        public virtual string Name { get { return "MemCacheProvider"; } }

        public bool Exists(string key)
        {
            object result;
            return new Enyim.Caching.MemcachedClient().TryGet(key, out result);
        }

        public CacheResult<T> Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value, int durationInSeconds)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
