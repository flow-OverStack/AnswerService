using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Interfaces.Provider;
using AutoMapper;
using Grpc.Core;

namespace AnswerService.GrpcClient.Providers;

public class QuestionProvider(QuestionService.QuestionServiceClient client, IMapper mapper)
    : IEntityProvider<QuestionDto>
{
    public async Task<QuestionDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var question = await client.GetQuestionByIdAsync(new GetQuestionByIdRequest { Id = id },
                cancellationToken: cancellationToken);
            return mapper.Map<QuestionDto>(question);
        }
        catch (RpcException e) when (e.Status.Detail == ErrorMessage.QuestionNotFound)
        {
            return null;
        }
    }
}