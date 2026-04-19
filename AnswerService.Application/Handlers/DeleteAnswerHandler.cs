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

public class DeleteAnswerHandler(
    IUnitOfWork unitOfWork,
    IEntityProvider<UserDto> userProvider,
    IBaseEventProducer producer,
    IMapper mapper) : IRequestHandler<DeleteAnswerCommand, BaseResult<AnswerDto>>
{
    public async Task<BaseResult<AnswerDto>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await unitOfWork.Answers.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (answer == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        if (!HasAccess(initiator, answer))
            return BaseResult<AnswerDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            answer.Enabled = false;
            unitOfWork.Answers.Update(answer);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await producer.ProduceAsync(answer.UserId, initiator.Id, answer.Id, BaseEventType.EntityDeleted,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }


        var answerDto = mapper.Map<AnswerDto>(answer);
        return BaseResult<AnswerDto>.Success(answerDto);
    }

    private static bool HasAccess(UserDto initiator, Answer toAnswer)
    {
        return initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Admin))
               || initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Moderator))
               || toAnswer.UserId == initiator.Id;
    }
}