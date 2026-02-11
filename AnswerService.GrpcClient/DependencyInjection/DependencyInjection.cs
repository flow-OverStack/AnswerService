using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.GrpcClient.Handlers;
using AnswerService.GrpcClient.Mappings;
using AnswerService.GrpcClient.Providers;
using AnswerService.GrpcClient.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnswerService.GrpcClient.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcClients(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GrpcMapping));
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<UserDto>, UserProvider>();
        services.AddScoped<IEntityProvider<QuestionDto>, QuestionProvider>();
        services.AddTransient<GrpcStatusMappingHandler>();

        var usersBuilder = services.AddGrpcClient<UserService.UserServiceClient>((provider, opt) =>
        {
            var usersHost = provider.GetRequiredService<IOptions<GrpcHosts>>().Value.UsersHost;
            opt.Address = new Uri(usersHost);
        });
        var questionsBuilder = services.AddGrpcClient<QuestionService.QuestionServiceClient>((provider, opt) =>
        {
            var questionsHost = provider.GetRequiredService<IOptions<GrpcHosts>>().Value.QuestionsHost;
            opt.Address = new Uri(questionsHost);
        });

        AddDefaultHandlers(usersBuilder, questionsBuilder);
    }

    private static void AddDefaultHandlers(params IHttpClientBuilder[] builders)
    {
        foreach (var builder in builders)
        {
            builder.AddStandardResilienceHandler();
            builder.AddHttpMessageHandler<GrpcStatusMappingHandler>();
        }
    }
}