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

public class AcceptAnswerHandler(
    IUnitOfWork unitOfWork,
    IEntityProvider<UserDto> userProvider,
    IEntityProvider<QuestionDto> questionProvider,
    IBaseEventProducer producer,
    IMapper mapper) : IRequestHandler<AcceptAnswerCommand, BaseResult<AnswerDto>>
{
    public async Task<BaseResult<AnswerDto>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await unitOfWork.Answers.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (answer == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        var question = await questionProvider.GetByIdAsync(answer.QuestionId, cancellationToken);
        // Impossible case because of foreign key constraint
        if (question == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (question.UserId != initiator.Id)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        if (answer.IsAccepted)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.AnswerAlreadyAccepted,
                (int)ErrorCodes.AnswerAlreadyAccepted);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            answer.IsAccepted = true;
            unitOfWork.Answers.Update(answer);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await producer.ProduceAsync(answer.UserId, initiator.Id, answer.Id, BaseEventType.EntityAccepted,
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
}