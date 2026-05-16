using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Queries.VoteType;
using AnswerService.Domain.Dto.Vote;
using AnswerService.Domain.Entities;
using AnswerService.GraphQl.DataLoaders;
using AnswerService.GraphQl.Helpers;
using AnswerService.GraphQl.Middlewares;
using MediatR;

namespace AnswerService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of paginated answers")]
    [UseCursorPagingValidationMiddleware]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Answer>> GetAnswers([Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllAnswersQuery();

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a answer by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Answer?> GetAnswer(long id, AnswerDataLoader answerDataLoader,
        CancellationToken cancellationToken)
    {
        var answer = await answerDataLoader.LoadAsync(id, cancellationToken);

        return answer;
    }

    [GraphQLDescription("Returns a list of paginated votes")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Vote>> GetAnswerVotes([Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllVotesQuery();

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote by id of the Answer that was voted and the user that voted")]
    [UseFiltering]
    [UseSorting]
    public async Task<Vote?> GetAnswerVote(long answerId, long userId, VoteDataLoader voteLoader,
        CancellationToken cancellationToken)
    {
        var dto = new VoteDto(answerId, userId);
        var vote = await voteLoader.LoadAsync(dto, cancellationToken);

        return vote;
    }

    [GraphQLDescription("Returns a list of paginated votes types")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<VoteType>> GetAnswerVoteTypes([Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllVoteTypesQuery();

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote type by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<VoteType?> GetAnswerVoteType(long id, VoteTypeDataLoader voteTypeLoader,
        CancellationToken cancellationToken)
    {
        var voteType = await voteTypeLoader.LoadAsync(id, cancellationToken);

        return voteType;
    }
}