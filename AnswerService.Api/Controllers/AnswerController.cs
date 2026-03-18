using System.Net;
using System.Security.Claims;
using AnswerService.Api.Controllers.Base;
using AnswerService.Api.Dto.Answer;
using AnswerService.Application.Commands.AnswerCommands;
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

    /// <summary>
    ///     Accepts an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to accept an answer:
    ///     PATCH {answerId}/accept
    /// </remarks>
    [HttpPatch("{answerId:long}/accept")]
    public async Task<ActionResult<BaseResult<AnswerDto>>> AcceptAnswer(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new AcceptAnswerCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Revokes acceptance of an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to revoke acceptance of an answer:
    ///     PATCH {answerId}/revoke-acceptance
    /// </remarks>
    [HttpPatch("{answerId:long}/revoke-acceptance")]
    public async Task<ActionResult<BaseResult<AnswerDto>>> RevokeAnswerAcceptance(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new RevokeAcceptanceCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Downvotes an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to downvote an answer:
    ///     PATCH {answerId}/downvote
    /// </remarks>
    [HttpPatch("{answerId:long}/downvote")]
    public async Task<ActionResult<BaseResult<VoteAnswerDto>>> DownvoteAnswer(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new DownvoteAnswerCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Upvotes an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to upvote an answer:
    ///     PATCH {answerId}/upvote
    /// </remarks>
    [HttpPatch("{answerId:long}/upvote")]
    public async Task<ActionResult<BaseResult<VoteAnswerDto>>> UpvoteAnswer(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new UpvoteAnswerCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Removes vote for an answer
    /// </summary>
    /// <param name="answerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to remove a vote for an answer:
    ///     DELETE {answerId}/vote
    /// </remarks>
    [HttpDelete("{answerId:long}/vote")]
    public async Task<ActionResult<BaseResult<VoteAnswerDto>>> RemoveVote(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new RemoveVoteCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }
}