using Bolt.Cache.UnitTests.Fixtures;
using NSubstitute;
using Xunit;

namespace Bolt.Cache.UnitTests
{
    public class CacheStore_Remove_Async_Should : IUseFixture<CacheStoreFixture>
    {
        private CacheStoreFixture _fixture;

        [Fact]
        public void Remove_Value_From_All_Providers()
        {
            const string key = "key";

            _fixture.Get().RemoveAsync(key);

            _fixture.InMemoryCacheProvider.Received(1).RemoveAsync(key);
            _fixture.RedisCacheProvider.Received(1).RemoveAsync(key);
        }

        public void SetFixture(CacheStoreFixture data)
        {
            _fixture = data;
        }
    }
}
