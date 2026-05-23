using AnswerService.Cache.Providers;
using AnswerService.Cache.Repositories;
using AnswerService.Cache.Settings;
using AnswerService.Domain.Interfaces.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AnswerService.Cache.DependencyInjection;

public static class DependencyInjection
{
    public static void AddCache(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>().Value;
            var configuration = new ConfigurationOptions
            {
                EndPoints = { { redisSettings.Host, redisSettings.Port } },
                Password = redisSettings.Password
            };

            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IDatabase>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        services.InitProviders();
        services.InitRepositories();
    }

    private static void InitProviders(this IServiceCollection services)
    {
        services.AddScoped<ICacheProvider, RedisCacheProvider>();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<AnswerCacheRepository>()
            .AddClasses(c => c.InExactNamespaceOf<AnswerCacheRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}