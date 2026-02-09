using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
    }
}