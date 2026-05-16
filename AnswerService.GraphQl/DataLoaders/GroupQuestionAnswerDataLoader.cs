using AnswerService.Application.Queries.Answer;
using AnswerService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class GroupQuestionAnswerDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Answer>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Answer>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetQuestionsAnswersQuery(keys);

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, Answer>>().ToLookup(_ => 0L, _ => default(Answer)!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Answer = y }))
            .ToLookup(x => x.Key, x => x.Answer);

        return lookup;
    }
}