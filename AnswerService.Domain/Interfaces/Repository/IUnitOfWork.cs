using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Database;

namespace AnswerService.Domain.Interfaces.Repository;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<Answer> Answers { get; set; }

    IBaseRepository<Vote> Votes { get; set; }

    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}