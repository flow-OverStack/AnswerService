using AnswerService.Cache.Settings;

namespace AnswerService.Tests.UnitTests.Configurations;

internal static class RedisSettingsConfiguration
{
    public static RedisSettings GetRedisSettingsConfiguration()
    {
        return new RedisSettings { TimeToLiveInSeconds = 300 };
    }
}