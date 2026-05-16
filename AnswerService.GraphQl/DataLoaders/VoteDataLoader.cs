using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Dto.Vote;
using AnswerService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.GraphQl.DataLoaders;

public class VoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<VoteDto, Vote>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<VoteDto, Vote>> LoadBatchAsync(IReadOnlyList<VoteDto> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetVotesQuery(keys);

        var result = await mediator.Send(query, cancellationToken);

        var dictionary = new Dictionary<VoteDto, Vote>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => new VoteDto(x.AnswerId, x.UserId), x => x);

        return dictionary.AsReadOnly();
    }
}