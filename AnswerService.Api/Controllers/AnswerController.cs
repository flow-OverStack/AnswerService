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
/// <response code="401">User is not authenticated</response>
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// <response code="201">Answer was created successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="404">User or question not found</response>
    /// <response code="409">Answer already exists for the question by the user</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="200">Answer was deleted successfully</response>
    /// <response code="403">User is not authorized to delete the answer</response>
    /// <response code="404">Answer or user not found</response>
    [HttpDelete("{answerId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="200">Answer was edited successfully</response>
    /// <response code="400">Validation failed (invalid property)</response>
    /// <response code="403">User is not authorized to edit the answer</response>
    /// <response code="404">Answer or user not found</response>
    [HttpPut("{answerId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="200">Answer was accepted successfully</response>
    /// <response code="403">User is not authorized to accept the answer</response>
    /// <response code="404">Answer, user, or question not found</response>
    /// <response code="409">Answer is already accepted or question already has an accepted answer</response>
    [HttpPatch("{answerId:long}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// <response code="200">Answer acceptance was revoked successfully</response>
    /// <response code="403">User is not authorized to revoke acceptance of the answer</response>
    /// <response code="404">Answer, user, or question not found</response>
    /// <response code="400">Answer is not accepted</response>
    [HttpPatch("{answerId:long}/revoke-acceptance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="403">User is  voting on their own post or has an insufficient reputation</response>
    /// <response code="404">User, answer or vote type not found</response>
    /// <response code="409">User has already voted on this answer</response>
    [HttpPatch("{answerId:long}/downvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// <response code="200">Vote was cast successfully</response>
    /// <response code="403">User is voting on their own post or has an insufficient reputation</response>
    /// <response code="404">User, answer or vote type not found</response>
    /// <response code="409">User has already voted on this answer</response
    [HttpPatch("{answerId:long}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// <response code="200">Vote was removed successfully</response>
    /// <response code="404">User, answer or vote not found</response>
    [HttpDelete("{answerId:long}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<VoteAnswerDto>>> RemoveVote(long answerId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var command = new RemoveVoteCommand(answerId, userId);

        var result = await mediator.Send(command, cancellationToken);

        return HandleBaseResult(result);
    }
}