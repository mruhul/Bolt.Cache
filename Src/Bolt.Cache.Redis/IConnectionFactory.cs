using StackExchange.Redis;

namespace Bolt.Cache.Redis
{
    public interface IConnectionFactory
    {
        ConnectionMultiplexer Create();
    }
}