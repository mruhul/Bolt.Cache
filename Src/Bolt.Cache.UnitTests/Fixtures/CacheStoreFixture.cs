using System.Collections.Generic;
using Bolt.Cache.Impl;
using Bolt.Logger;
using NSubstitute;

namespace Bolt.Cache.UnitTests.Fixtures
{
    public class CacheStoreFixture
    {
        internal ICacheProvider InMemoryCacheProvider { get; private set; }
        internal ICacheProvider RedisCacheProvider { get; private set; }
        internal ICacheSettingsProvider SettingsProvider { get; private set; }

        internal List<ICacheProvider> Providers { get; private set; } 

        public CacheStoreFixture()
        {
            InMemoryCacheProvider = Substitute.For<ICacheProvider>();
            InMemoryCacheProvider.Name.Returns("InMemory");
            InMemoryCacheProvider.Order.Returns(0);

            RedisCacheProvider = Substitute.For<ICacheProvider>();
            RedisCacheProvider.Name.Returns("Redis");
            RedisCacheProvider.Order.Returns(1);

            Providers = new List<ICacheProvider>
            {
                InMemoryCacheProvider,
                RedisCacheProvider
            };

            SettingsProvider = Substitute.For<ICacheSettingsProvider>();
        }

        public ICacheStore Get()
        {
            return new CacheStore(Providers, SettingsProvider, Substitute.For<ILogger>());
        }
    }
}