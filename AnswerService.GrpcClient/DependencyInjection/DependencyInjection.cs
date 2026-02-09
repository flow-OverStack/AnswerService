using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GrpcClient.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcClients(this IServiceCollection services)
    {
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
    }
}