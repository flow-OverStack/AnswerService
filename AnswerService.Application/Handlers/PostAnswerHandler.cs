using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers;

public class PostAnswerHandler(
    IBaseRepository<Answer> answerRepository,
    IEntityProvider<UserDto> userProvider,
    IEntityProvider<QuestionDto> questionProvider,
    IMapper mapper)
    : IRequestHandler<PostAnswerCommand, BaseResult<AnswerDto>>
{
    public async Task<BaseResult<AnswerDto>> Handle(PostAnswerCommand request, CancellationToken cancellationToken)
    {
        var user = await userProvider.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await questionProvider.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        var answer = await answerRepository.GetAll()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.QuestionId == request.QuestionId,
                cancellationToken);
        if (answer != null)
            return BaseResult<AnswerDto>.Failure(ErrorMessage.AnswerAlreadyExists, (int)ErrorCodes.AnswerAlreadyExists);

        answer = mapper.Map<Answer>(request);
        await answerRepository.CreateAsync(answer, cancellationToken);
        await answerRepository.SaveChangesAsync(cancellationToken);

        var answerDto = mapper.Map<AnswerDto>(answer);
        return BaseResult<AnswerDto>.Success(answerDto);
    }
}