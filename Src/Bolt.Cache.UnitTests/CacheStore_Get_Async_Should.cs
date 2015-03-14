using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.Cache.Dto;
using Bolt.Cache.Extensions;
using Bolt.Cache.UnitTests.Fixtures;
using NSubstitute;
using Should;
using Xunit;

namespace Bolt.Cache.UnitTests
{
    public class CacheStore_Get_Async_Should : IUseFixture<CacheStoreFixture>
    {
        private CacheStoreFixture _fixture;

        [Fact]
        public void Return_Value_When_No_Providers_Registerd()
        {
            _fixture.Providers.Clear();

            var resultAsync =
                _fixture.Get().Profile("").FetchAsync(() => Task.FromResult("Hello World")).GetAsync("Test");

            resultAsync.Result.ShouldEqual("Hello World");
        }

        [Fact]
        public void Return_Value_When_No_Providers_Has_CacheValue()
        {
            var inMemoryName = _fixture.InMemoryCacheProvider.Name;
            var redisName = _fixture.RedisCacheProvider.Name;


            _fixture.SettingsProvider.Get(inMemoryName + ".Short")
                .Returns(new CacheSettings
                {
                    Disabled = false,
                    DurationInSeconds = 5,
                    Name = inMemoryName + ".Short"
                });

            _fixture.SettingsProvider.Get(redisName + ".Short")
                .Returns(new CacheSettings
                {
                    Disabled = false,
                    DurationInSeconds = 10,
                    Name = redisName + ".Short"
                });

            _fixture.InMemoryCacheProvider.GetAsync<string>("Test")
                .Returns(Task.FromResult(new CacheResult<string>(false, null)));
            _fixture.RedisCacheProvider.GetAsync<string>("Test")
                .Returns(Task.FromResult(new CacheResult<string>(false, null)));

            var resultAsync =
                _fixture.Get().Profile("Short")
                .FetchAsync(() => Task.FromResult("Hello World"))
                .GetAsync("Test");


            resultAsync.Result.ShouldEqual("Hello World");
            _fixture.InMemoryCacheProvider.Received(1).SetAsync("Test", "Hello World", 5);
            _fixture.RedisCacheProvider.Received(1).SetAsync("Test","Hello World",10);
        }

        [Fact]
        public void Set_Cache_Value_If_Top_Stack_Provider_Miss_Cache()
        {
            var inMemoryName = _fixture.InMemoryCacheProvider.Name;
            var redisName = _fixture.RedisCacheProvider.Name;


            _fixture.SettingsProvider.Get(inMemoryName + ".Short")
                .Returns(new CacheSettings
                {
                    Disabled = false,
                    DurationInSeconds = 5,
                    Name = inMemoryName + ".Short"
                });

            _fixture.SettingsProvider.Get(redisName + ".Short")
                .Returns(new CacheSettings
                {
                    Disabled = false,
                    DurationInSeconds = 10,
                    Name = redisName + ".Short"
                });

            _fixture.InMemoryCacheProvider.GetAsync<string>("Test")
                .Returns(Task.FromResult(new CacheResult<string>(false, null)));
            _fixture.RedisCacheProvider.GetAsync<string>("Test")
                .Returns(Task.FromResult(new CacheResult<string>(true, "Hello World!")));

            var resultAsync =
                _fixture.Get().Profile("Short")
                .FetchAsync(() => Task.FromResult("Hello World"))
                .GetAsync("Test");


            resultAsync.Result.ShouldEqual("Hello World!");
            _fixture.InMemoryCacheProvider.Received(1).SetAsync("Test", "Hello World!", 5);
        }

        public void SetFixture(CacheStoreFixture data)
        {
            _fixture = data;
        }
    }

    internal class CacheSettings : ICacheSetting
    {
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public int DurationInSeconds { get; set; }
    }
}