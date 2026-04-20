using AnswerService.Api.Settings;
using AnswerService.Cache.Settings;
using AnswerService.DAL;
using AnswerService.GrpcClient;
using AnswerService.GrpcClient.Settings;
using AnswerService.Outbox.Events;
using AnswerService.Tests.FunctionalTests.Configurations;
using AnswerService.Tests.FunctionalTests.Configurations.TestServices;
using AnswerService.Tests.FunctionalTests.Extensions;
using AnswerService.Tests.FunctionalTests.Helper;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WireMock.Server;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Base;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _answerServicePostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("answer-service-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private WireMockServer _wireMockServer = null!;

    public async Task InitializeAsync()
    {
        await _answerServicePostgreSql.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _answerServicePostgreSql.StopAsync();
        await _redisContainer.StopAsync();
        _wireMockServer.StopServer();
        _wireMockServer.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:PostgresSQL", _answerServicePostgreSql.GetConnectionString() }
                }!)
                .Build();

            config.AddConfiguration(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            var connectionString = _answerServicePostgreSql.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();
            scope.PrepPopulation();

            _wireMockServer = _wireMockServer.StartIdentityServer();

            services.RemoveAll<IOptions<KeycloakSettings>>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Host = _wireMockServer.Url!;
                x.Realm = WireMockIdentityServerExtensions.RealmName;
                x.Audience = TokenHelper.GetAudience();
            });

            services.RemoveAll<IOptions<GrpcHosts>>();
            services.Configure<GrpcHosts>(x =>
            {
                x.UsersHost = "TestUsersHost";
                x.QuestionsHost = "TestQuestionsHost";
            });

            services.RemoveAll<UserService.UserServiceClient>();
            services.AddSingleton<UserService.UserServiceClient, GrpcTestUserService>();
            services.RemoveAll<GrpcClient.QuestionService.QuestionServiceClient>();
            services.AddSingleton<GrpcClient.QuestionService.QuestionServiceClient, GrpcTestQuestionService>();

            services.RemoveAll<ITopicProducer<BaseEvent>>();
            services.AddScoped<ITopicProducer<BaseEvent>, TestTopicProducer<BaseEvent>>();

            services.RemoveAll<IOptions<RedisSettings>>();
            services.Configure<RedisSettings>(x =>
            {
                _redisContainer.GetConnectionString().ParseConnectionString(out var host, out var port);
                x.Host = host;
                x.Port = port;
                x.Password = null!;
            });
        });
    }
}