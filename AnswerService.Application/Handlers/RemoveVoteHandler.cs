using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Producer;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers;

public class RemoveVoteHandler(
    IUnitOfWork unitOfWork,
    IEntityProvider<UserDto> userProvider,
    IBaseEventProducer producer,
    IMapper mapper) : IRequestHandler<RemoveVoteCommand, BaseResult<VoteAnswerDto>>
{
    public async Task<BaseResult<VoteAnswerDto>> Handle(RemoveVoteCommand request, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await unitOfWork.Answers.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (answer == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        var vote = answer.Votes.FirstOrDefault(x => x.UserId == initiator.Id);
        if (vote == null)
            return BaseResult<VoteAnswerDto>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            unitOfWork.Votes.Remove(vote);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await producer.ProduceAsync(answer.UserId, initiator.Id, answer.Id, BaseEventType.EntityVoteRemoved,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var voteAnswerDto = mapper.Map<VoteAnswerDto>(answer);
        return BaseResult<VoteAnswerDto>.Success(voteAnswerDto);
    }
}