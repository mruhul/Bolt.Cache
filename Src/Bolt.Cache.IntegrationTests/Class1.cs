﻿using Bolt.Cache.Builders;
using Bolt.Cache.Extensions;
using Bolt.Cache.Impl;
using Bolt.Cache.Redis.Builders;
using Bolt.Logger;
using Bolt.Serializer;
using Bolt.Serializer.Json;
using Ninject;
using Ninject.Modules;
using Xunit;

namespace Bolt.Cache.IntegrationTests
{
    public class CacheStore_Should
    {
        [Fact]
        public void Cache_All_Registered_Services()
        {
            var kernel = new StandardKernel();
            kernel.Load(new FluentCacheModule());

            var cacheStore = kernel.Get<ICacheStore>();

            const string key = "Helloworld";

            var  result = cacheStore.Profile("Short")
                                .Fetch<string>(() => string.Empty)
                                .Get(key);


            var result2 = cacheStore.Profile("Short")
                                .Fetch<string>(() => string.Empty)
                                .Get(key);

            Assert.DoesNotThrow(() => cacheStore.Remove(key));
        }
    }

    public class FluentCacheModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISerializer>().To<JsonSerializer>().InSingletonScope();

            Bind<ICacheProvider>().ToMethod(x => InMemoryCacheProvider.Default).InSingletonScope();

            Bind<ICacheProvider>().ToMethod(x => 
                RedisCacheProviderBuilder.New().Serializer(x.Kernel.Get<ISerializer>()).Build())
                .InSingletonScope();

            Bind<ICacheStore>().ToMethod(x => CacheStoreBuilder.New().Build());
        }
    }

    public class CacheModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().ToMethod(x => Bolt.Logger.NLog.LoggerFactory.Create(x.Request.Target.GetType()));
            Bind<ICacheSettingsProvider>()
                .ToMethod(x => new ConfigBasedCacheSettingsProvider("CacheSettings"))
                .InSingletonScope();
            Bind<ICacheProvider>().To<InMemoryCacheProvider>()
                .WithConstructorArgument("name", "InMemory")
                .WithConstructorArgument("order", 0);

            Bind<Bolt.Cache.Redis.IConnectionSettings>()
                .ToMethod(x => Bolt.Cache.Redis.Configs.ConnectionSettingsSection
                                .Instance("RedisSettings"))
                .InSingletonScope();
            Bind<Bolt.Serializer.ISerializer>().To<Bolt.Serializer.Json.JsonSerializer>().InSingletonScope();
            Bind<Bolt.Cache.Redis.IConnectionFactory>().To<Bolt.Cache.Redis.ConnectionFactory>().InSingletonScope();

            Bind<ICacheProvider>().To<Bolt.Cache.Redis.RedisCacheProvider>()
                .WithConstructorArgument("name", "Redis")
                .WithConstructorArgument("order", 1);
            

            
            Bind<ICacheStore>().To<CacheStore>().InSingletonScope();
        }
    }
}
