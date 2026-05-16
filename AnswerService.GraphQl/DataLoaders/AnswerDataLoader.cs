using AnswerService.Application.Queries.Answer;
using AnswerService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class AnswerDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, Answer>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, Answer>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetAnswersQuery(keys);

        var result = await mediator.Send(query, cancellationToken);

        var dictionary = new Dictionary<long, Answer>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}