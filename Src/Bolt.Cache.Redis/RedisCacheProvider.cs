using System;
using Bolt.Cache.Dto;
using Bolt.Serializer;
using StackExchange.Redis;

namespace Bolt.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly string _name;
        private readonly int _order;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISerializer _serializer;
        private readonly IConnectionSettings _settings; 

        public RedisCacheProvider(string name, int order,
            IConnectionSettings settings,
            IConnectionFactory connectionFactory,
            ISerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
            _settings = settings;
            _name = name;
            _order = order;
        }

        private IDatabase Database
        {
            get { return _connectionFactory.Create().GetDatabase(_settings.Database); }
        }

        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        public CacheResult<T> Get<T>(string key)
        {
            var result = Database.StringGet(key, CommandFlags.PreferSlave);

            return result.HasValue 
                    ? new CacheResult<T>(true, _serializer.Deserialize<T>(result)) 
                    : CacheResult<T>.Empty ;
        }

        public void Set<T>(string key, T value, int durationInSeconds)
        {
            Database.StringSet(key, _serializer.Serialize(value), TimeSpan.FromSeconds(durationInSeconds));
        }

        public void Remove(string key)
        {
            Database.KeyDelete(key);
        }

        public int Order { get { return _order; } }
        public string Name { get { return _name; } }
    }
}
