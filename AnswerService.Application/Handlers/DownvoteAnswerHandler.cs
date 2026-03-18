using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
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

public class DownvoteAnswerHandler(
    IUnitOfWork unitOfWork,
    IBaseRepository<VoteType> voteTypeRepository,
    IEntityProvider<UserDto> userProvider,
    IBaseEventProducer producer,
    IMapper mapper) : IRequestHandler<DownvoteAnswerCommand, BaseResult<VoteAnswerDto>>
{
    public async Task<BaseResult<VoteAnswerDto>> Handle(DownvoteAnswerCommand request,
        CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await unitOfWork.Answers.GetAll()
            .Include(x => x.Votes)
            .ThenInclude(x => x.VoteType)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (answer == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        if (initiator.Id == answer.UserId)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.CannotVoteForOwnPost,
                (int)ErrorCodes.CannotVoteForOwnPost);

        var vote = answer.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        var voteType = await voteTypeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == nameof(VoteTypes.Downvote), cancellationToken);
        if (voteType == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.VoteTypeNotFound, (int)ErrorCodes.VoteTypeNotFound);

        if (initiator.Reputation < voteType.MinReputationToVote)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
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
                if (vote.VoteType.Id == voteType.Id)
                    return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                        (int)ErrorCodes.VoteAlreadyGiven);

                vote.VoteType = voteType;
                unitOfWork.Votes.Update(vote);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await producer.ProduceAsync(answer.UserId, initiator.Id, answer.Id, BaseEventType.EntityDownvoted,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

        var dto = mapper.Map<VoteAnswerDto>(answer);

        return BaseResult<VoteAnswerDto>.Success(dto);
    }
}