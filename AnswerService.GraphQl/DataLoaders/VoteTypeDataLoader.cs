using AnswerService.Application.Queries.VoteType;
using AnswerService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class VoteTypeDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, VoteType>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, VoteType>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetVoteTypesQuery(keys);

        var result = await mediator.Send(query, cancellationToken);

        var dictionary = new Dictionary<long, VoteType>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}