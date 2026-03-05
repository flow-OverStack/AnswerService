using System.Net;
using System.Security.Claims;
using AnswerService.Api.Controllers.Base;
using AnswerService.Api.Dto.Answer;
using AnswerService.Application.Commands;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Api.Controllers;

/// <summary>
///     Answer controller
/// </summary>
/// <response code="200">If answer was posted/edited/deleted</response>
/// <response code="400">If answer was not posted/edited/deleted</response>
/// <response code="403">If the operation was forbidden for user</response>
/// <response code="500">If internal server error occurred</response>
[Authorize]
public class AnswerController(IMediator mediator) : BaseController
{
    /// <summary>
    ///     Creates an answer
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to post an answer:
    ///     POST
    ///     {
    ///     "answerId": 0,
    ///     "body": string
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<AnswerDto>>> PostAnswer(PostAnswerDto dto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new PostAnswerCommand(dto.Body, userId, dto.QuestionId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Deletes an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to delete an answer:
    ///     DELETE {answerId}
    /// </remarks>
    [HttpDelete("{answerId:long}")]
    public async Task<ActionResult<BaseResult<AnswerDto>>> DeleteAnswer(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new DeleteAnswerCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Edits an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to edit an answer:
    ///     PUT
    ///     {
    ///     "body": "string"
    ///     }
    /// </remarks>
    [HttpPut("{answerId:long}")]
    public async Task<ActionResult<BaseResult<AnswerDto>>> EditAnswer(long answerId, EditAnswerDto dto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new EditAnswerCommand(answerId, dto.Body, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }
}