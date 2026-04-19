using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers;

public class EditAnswerHandler(
    IBaseRepository<Answer> answerRepository,
    IEntityProvider<UserDto> userProvider,
    IMapper mapper) : IRequestHandler<EditAnswerCommand, BaseResult<AnswerDto>>
{
    public async Task<BaseResult<AnswerDto>> Handle(EditAnswerCommand request, CancellationToken cancellationToken)
    {
        var initiator = await userProvider.GetByIdAsync(request.InitiatorId, cancellationToken);
        if (initiator == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var answer = await answerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (answer == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.AnswerNotFound, (int)ErrorCodes.AnswerNotFound);

        if (!HasAccess(initiator, answer))
            return BaseResult<AnswerDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        mapper.Map(request, answer);

        answerRepository.Update(answer);
        await answerRepository.SaveChangesAsync(cancellationToken);

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