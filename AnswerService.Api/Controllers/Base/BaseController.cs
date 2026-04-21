using System.Net;
using System.Net.Mime;
using AnswerService.Application.Enum;
using AnswerService.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Api.Controllers.Base;

/// <inheritdoc />
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class BaseController : ControllerBase
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
    ///     Handles the BaseResult of type T and returns the corresponding ActionResult
    /// </summary>
    /// <param name="result"></param>
    /// <param name="successStatusCode"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    protected ActionResult<BaseResult<T>> HandleBaseResult<T>(
        BaseResult<T> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK) where T : class
    {
        var statusCode = GetStatusCode(result.IsSuccess, result.ErrorCode, (int)successStatusCode);
        return StatusCode(statusCode, result);
    }


    private static int GetStatusCode(bool isSuccess, int? errorCode, int successStatusCode)
    {
        const int defaultCode = StatusCodes.Status400BadRequest;

        if (isSuccess) return successStatusCode;
        if (errorCode == null || !ErrorStatusCodeMap.TryGetValue((int)errorCode, out var code)) return defaultCode;
        return code;
    }
}