using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.GrpcClient;
using AnswerService.GrpcClient.Mappings;
using AnswerService.Tests.Configurations;
using AutoMapper;
using Grpc.Core;

namespace AnswerService.Tests.FunctionalTests.Configurations.TestServices;

internal class GrpcTestQuestionService : GrpcClient.QuestionService.QuestionServiceClient
{
    private static readonly IEnumerable<QuestionDto> Questions = MockEntityProvidersGetters.GetQuestionDtos();

    private static readonly IMapper Mapper =
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(GrpcMapping))).CreateMapper();

    public override GrpcQuestion GetQuestionById(GetQuestionByIdRequest request, CallOptions options)
    {
        return GetQuestionById(request.Id);
    }

    public override GrpcQuestion GetQuestionById(GetQuestionByIdRequest request, Metadata headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return GetQuestionById(request.Id);
    }

    public override AsyncUnaryCall<GrpcQuestion> GetQuestionByIdAsync(GetQuestionByIdRequest request,
        Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return ToAsyncUnaryCall(GetQuestionById(request.Id));
    }

    public override AsyncUnaryCall<GrpcQuestion> GetQuestionByIdAsync(GetQuestionByIdRequest request,
        CallOptions options)
    {
        return ToAsyncUnaryCall(GetQuestionById(request.Id));
    }

    private static AsyncUnaryCall<T> ToAsyncUnaryCall<T>(T response)
    {
        var responseTask = Task.FromResult(response);
        var metadataTask = Task.FromResult(new Metadata());

        return new AsyncUnaryCall<T>(
            responseTask,
            metadataTask,
            () => Status.DefaultSuccess,
            () => [],
            () => { });
    }

    private static GrpcQuestion GetQuestionById(long id)
    {
        var question = Questions.FirstOrDefault(x => x.Id == id);

        if (question == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.QuestionNotFound),
                new Metadata { { "ErrorCode", nameof(ErrorCodes.QuestionNotFound) } });

        return Mapper.Map<GrpcQuestion>(question);
    }
}