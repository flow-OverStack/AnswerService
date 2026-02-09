using AnswerService.DAL.Repositories;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Outbox.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.InitRepositories();
    }

    /// <summary>
    ///     Migrates the database
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Answer>, BaseRepository<Answer>>();
        services.AddScoped<IBaseRepository<Vote>, BaseRepository<Vote>>();
        services.AddScoped<IBaseRepository<VoteType>, BaseRepository<VoteType>>();
        services.AddScoped<IBaseRepository<OutboxMessage>, BaseRepository<OutboxMessage>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}