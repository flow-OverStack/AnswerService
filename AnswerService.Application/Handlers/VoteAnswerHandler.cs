using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Enums;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dtos.Answer;
using AnswerService.Domain.Dtos.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Producer;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers;

public class VoteAnswerHandler(
    IUnitOfWork unitOfWork,
    IBaseRepository<VoteType> voteTypeRepository,
    IEntityProvider<UserDto> userProvider,
    IBaseEventProducer producer,
    IMapper mapper) :
    IRequestHandler<UpvoteAnswerCommand, BaseResult<VoteAnswerDto>>,
    IRequestHandler<DownvoteAnswerCommand, BaseResult<VoteAnswerDto>>
{
    public Task<BaseResult<VoteAnswerDto>> Handle(UpvoteAnswerCommand request, CancellationToken cancellationToken) =>
        VoteAsync(request.InitiatorId, request.Id, VoteTypes.Upvote, BaseEventType.EntityUpvoted, cancellationToken);

    public Task<BaseResult<VoteAnswerDto>> Handle(DownvoteAnswerCommand request,
        CancellationToken cancellationToken) =>
        VoteAsync(request.InitiatorId, request.Id, VoteTypes.Downvote, BaseEventType.EntityDownvoted,
            cancellationToken);

    private async Task<BaseResult<VoteAnswerDto>> VoteAsync(long initiatorId, long answerId, VoteTypes voteTypeName,
        BaseEventType eventType, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await unitOfWork.Answers.GetAll()
            .Include(x => x.Votes)
            .ThenInclude(x => x.VoteType)
            .FirstOrDefaultAsync(x => x.Id == answerId, cancellationToken);
        if (answer == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        if (initiator.Id == answer.UserId)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.CannotVoteForOwnPost,
                (int)ErrorCodes.CannotVoteForOwnPost);

        var vote = answer.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        var voteTypeNameString = voteTypeName.ToString();
        var voteType = await voteTypeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == voteTypeNameString, cancellationToken);
        if (voteType == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound);

        if (initiator.Reputation < voteType.MinReputationToVote)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        if (vote != null && vote.VoteType.Id == voteType.Id)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.VoteAlreadyGiven, (int)ErrorCodes.VoteAlreadyGiven);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        if (vote == null)
        {
            vote = new Vote
            {
                AnswerId = answer.Id,
                UserId = initiator.Id,
                VoteType = voteType
            };

            await unitOfWork.Votes.CreateAsync(vote, cancellationToken);
        }
        else
        {
            vote.VoteType = voteType;
            unitOfWork.Votes.Update(vote);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await producer.ProduceAsync(answer.UserId, initiator.Id, answer.Id, eventType, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var dto = mapper.Map<VoteAnswerDto>(answer);

        return BaseResult<VoteAnswerDto>.Success(dto);
    }
}
