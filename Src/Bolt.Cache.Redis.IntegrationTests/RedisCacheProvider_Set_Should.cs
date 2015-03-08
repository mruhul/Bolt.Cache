using System;
using System.Threading;
using Should;
using Xunit;

namespace Bolt.Cache.Redis.IntegrationTests
{
    public class RedisCacheProvider_Set_Should : IUseFixture<RedisFixture>
    {
        private Bolt.Cache.ICacheProvider _cacheProvider;

        [Fact]
        public void Skip_Storing_Null_Object()
        {
            const string Key = "EmptyBook";
            Book emptyBook = null;
            _cacheProvider.Set(Key, emptyBook, 1);

            var result = _cacheProvider.Get<Book>(Key);

            result.Available.ShouldBeFalse();
            result.Value.ShouldBeNull();
        }

        [Fact]
        public void Set_Should_Save()
        {
            var book = new Book { Name = "Clean code" };

            _cacheProvider.Set("books-cleancode", book, 5);

            var record = _cacheProvider.Get<Book>("books-cleancode");

            record.Available.ShouldBeTrue();
            record.Value.Name.ShouldEqual("Clean code");
        }

        [Fact]
        public void Get_Should_Return_NotAvailable_After_Expiry()
        {
            var book = new Book { Name = "Clean code" };

            _cacheProvider.Set("books-cleancode", book, 1);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var record = _cacheProvider.Get<Book>("books-cleancode");

            record.Available.ShouldBeFalse();
            record.Value.ShouldBeNull();
        }

        public void SetFixture(RedisFixture data)
        {
            _cacheProvider = data.Get();
        }
    }

    public class Book
    {
        public string Name { get; set; }
    }
}
