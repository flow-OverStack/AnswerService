using System.Net;
using AnswerService.Application.Enums;
using AnswerService.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Api.Extensions;

public static class BaseResultExtensions
{
    private static readonly IReadOnlyDictionary<int, int> ErrorStatusCodeMap = new Dictionary<int, int>
    {
        // Data
        { (int)ErrorCodes.InvalidProperty, StatusCodes.Status400BadRequest },

        // User
        { (int)ErrorCodes.UserNotFound, StatusCodes.Status404NotFound },

        // Question
        { (int)ErrorCodes.QuestionNotFound, StatusCodes.Status404NotFound },

        // Answer
        { (int)ErrorCodes.AnswerNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.AnswerAlreadyExists, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.AnswerAlreadyAccepted, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.AnswerNotAccepted, StatusCodes.Status400BadRequest },
        { (int)ErrorCodes.QuestionAlreadyHasAcceptedAnswer, StatusCodes.Status409Conflict },

        // Authorization
        { (int)ErrorCodes.OperationForbidden, StatusCodes.Status403Forbidden },

        // Votes
        { (int)ErrorCodes.VoteAlreadyGiven, StatusCodes.Status409Conflict },
        { (int)ErrorCodes.VoteNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.VotesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.VoteTypeNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.VoteTypesNotFound, StatusCodes.Status404NotFound },
        { (int)ErrorCodes.CannotVoteForOwnPost, StatusCodes.Status422UnprocessableEntity }
    };

    /// <summary>
    ///     Converts a BaseResult of type T into the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    public static ActionResult<BaseResult<T>> ToActionResult<T>(
        this BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        if (result.IsSuccess) return new ObjectResult(result) { StatusCode = (int)successStatusCode };

        return new ObjectResult(result) { StatusCode = GetStatusCode(result.ErrorCode) };
    }

    private static int GetStatusCode(int? errorCode)
    {
        if (errorCode != null && ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return code;

        return StatusCodes.Status500InternalServerError;
    }
}