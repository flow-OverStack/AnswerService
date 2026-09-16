using AnswerService.Application.Queries.Answer;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Results;
using AnswerService.GraphQl.DataLoaders.Base;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class GroupQuestionAnswerDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<Answer, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<Answer>>>> FetchAsync(
        IServiceProvider scopedProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IMediator>().Send(new GetQuestionsAnswersQuery(keys), cancellationToken);
}
