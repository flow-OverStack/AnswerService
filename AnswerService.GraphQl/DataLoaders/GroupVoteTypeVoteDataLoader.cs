using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Results;
using AnswerService.GraphQl.DataLoaders.Base;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class GroupVoteTypeVoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<Vote, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> FetchAsync(
        IServiceProvider scopedProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IMediator>().Send(new GetVoteTypesVotesQuery(keys), cancellationToken);
}
