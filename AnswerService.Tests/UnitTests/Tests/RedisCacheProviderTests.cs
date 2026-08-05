using AnswerService.Cache.Providers;
using AnswerService.Tests.UnitTests.Fixtures;
using StackExchange.Redis;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class RedisCacheProviderTests
{
    [Fact]
    public async Task SetsAdd_KeyExpireReturnsFalse_ThrowsRedisException()
    {
        //Arrange
        var cache = new RedisCacheProvider(
            RedisDatabaseFixture.GetRedisDatabaseConfiguration());
        var keysWithValues = new KeyValuePair<string, IEnumerable<string>>[]
        {
            new("key1", ["value11", "value12"]),
            new("key2", ["value21", "value22"]),
            new("key3", ["value31", "value32"])
        };

        //Act
        var action = async () => await cache.SetsAddAsync(keysWithValues, int.MaxValue);

        //Assert
        await Assert.ThrowsAsync<RedisException>(action);
    }

    [Fact]
    public async Task StringSet_RedisReturnsFalse_ThrowsRedisException()
    {
        //Arrange
        var cache = new RedisCacheProvider(
            RedisDatabaseFixture.GetFalseResponseRedisDatabaseConfiguration());
        var keysWithValues = new KeyValuePair<string, object>[]
        {
            new("key1", "value1"),
            new("key2", "value2"),
            new("key3", "value3")
        };

        //Act
        var action = async () => await cache.StringSetAsync(keysWithValues, int.MaxValue);

        //Assert
        await Assert.ThrowsAsync<RedisException>(action);
    }
}