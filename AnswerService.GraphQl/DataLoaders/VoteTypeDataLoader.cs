using AnswerService.Application.Queries.VoteType;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Results;
using AnswerService.GraphQl.DataLoaders.Base;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class VoteTypeDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<VoteType, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<VoteType>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IMediator>().Send(new GetVoteTypesQuery(keys), cancellationToken);

    protected override long GetId(VoteType entity) => entity.Id;
}
