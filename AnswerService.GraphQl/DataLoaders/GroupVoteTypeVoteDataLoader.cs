using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class GroupVoteTypeVoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Vote>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Vote>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetVoteTypesVotesQuery(keys);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, Vote>>().ToLookup(_ => 0L, _ => default(Vote)!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Vote = y }))
            .ToLookup(x => x.Key, x => x.Vote);

        return lookup;
    }
}