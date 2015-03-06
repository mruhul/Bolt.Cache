using System.Collections.Generic;
using Bolt.Cache.Configs;
using Bolt.Cache.Extensions;
using Bolt.Cache.Impl;
using Bolt.Cache.Redis;
using Bolt.Cache.Redis.Configs;
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
            kernel.Load(new CacheModule());

            var cacheStore = kernel.Get<ICacheStore>();

            const string key = "Helloworld";

            var  result = cacheStore.Profile("Short")
                                .Fetch<string>(() => string.Empty)
                                .Get(key);


            var result2 = cacheStore.Profile("Short")
                                .Fetch<string>(() => string.Empty)
                                .Get(key);

            cacheStore.Remove(key);
        }
    }

    public class CacheModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICacheSettingsProvider>()
                .ToMethod(x => new ConfigBasedCacheSettingsProvider("CacheSettings"))
                .InSingletonScope();
            Bind<ICacheProvider>().To<InMemoryCacheProvider>()
                .WithConstructorArgument("name", "InMemory")
                .WithConstructorArgument("priority", 2);

            Bind<Bolt.Cache.Redis.IConnectionSettings>()
                .ToMethod(x => Bolt.Cache.Redis.Configs.ConnectionSettingsSection
                                .Instance("RedisSettings"))
                .InSingletonScope();
            Bind<Bolt.Serializer.ISerializer>().To<Bolt.Serializer.Json.JsonSerializer>().InSingletonScope();
            Bind<Bolt.Cache.Redis.IConnectionFactory>().To<Bolt.Cache.Redis.ConnectionFactory>().InSingletonScope();

            Bind<ICacheProvider>().To<Bolt.Cache.Redis.CacheProvider>()
                .WithConstructorArgument("name", "Redis")
                .WithConstructorArgument("priority", 1);
            

            
            Bind<ICacheStore>().To<CacheStore>().InSingletonScope();
        }
    }
}
