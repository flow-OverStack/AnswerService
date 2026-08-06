using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Dtos.Vote;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Results;
using AnswerService.GraphQl.DataLoaders.Base;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class VoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Vote, VoteDto>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Vote>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<VoteDto> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IMediator>().Send(new GetVotesQuery(keys), cancellationToken);

    protected override VoteDto GetId(Vote entity) => new(entity.AnswerId, entity.UserId);
}
