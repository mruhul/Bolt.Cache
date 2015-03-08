using Bolt.Serializer.Json;

namespace Bolt.Cache.Redis.IntegrationTests
{
    public class RedisFixture
    {
        public ICacheProvider Get()
        {
            return Bolt.Cache.Redis.Builders
                .RedisCacheProviderBuilder
                .New()
                .Serializer(new JsonSerializer())
                .Build();
        } 
    }
}