using AnswerService.Application.Queries.Answer;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Results;
using AnswerService.GraphQl.DataLoaders.Base;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class AnswerDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Answer, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Answer>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IMediator>().Send(new GetAnswersQuery(keys), cancellationToken);

    protected override long GetId(Answer entity) => entity.Id;
}
