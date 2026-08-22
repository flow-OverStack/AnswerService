using AnswerService.Application.Enums;
using AnswerService.Application.Resources;
using AnswerService.Domain.Results;

namespace AnswerService.Application.Extensions;

public static class CollectionResultExtensions
{
    extension<T>(CollectionResult<T>)
    {
        public static CollectionResult<T> AnswersNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.AnswersNotFound, (int)ErrorCodes.AnswersNotFound)
        };

        public static CollectionResult<T> VotesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
        };

        public static CollectionResult<T> VoteTypesNotFound(int requestedCount) => requestedCount switch
        {
            <= 1 => CollectionResult<T>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound),
            > 1 => CollectionResult<T>.Failure(ErrorMessage.VoteTypesNotFound, (int)ErrorCodes.VoteTypesNotFound)
        };
    }
}