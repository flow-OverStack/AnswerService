using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Database;
using AnswerService.Domain.Interfaces.Repository;

namespace AnswerService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<Answer> answers,
    IBaseRepository<Vote> votes)
    : IUnitOfWork
{
    public IBaseRepository<Answer> Answers { get; set; } = answers;

    public IBaseRepository<Vote> Votes { get; set; } = votes;

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new DbContextTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}