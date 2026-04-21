using AnswerService.Application.Enum;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.GrpcClient;
using AnswerService.GrpcClient.Mappings;
using AnswerService.Tests.Configurations;
using AutoMapper;
using Grpc.Core;

namespace AnswerService.Tests.FunctionalTests.Configurations.TestServices;

internal class GrpcTestUserService : UserService.UserServiceClient
{
    private static readonly IEnumerable<UserDto> Users = MockEntityProvidersGetters.GetUserDtos();

    private static readonly IMapper Mapper =
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(GrpcMapping))).CreateMapper();

    public override GrpcUser GetUserWithRolesById(GetUserByIdRequest request, CallOptions options)
    {
        return GetUserById(request.Id);
    }

    public override GrpcUser GetUserWithRolesById(GetUserByIdRequest request, Metadata? headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return GetUserById(request.Id);
    }

    public override AsyncUnaryCall<GrpcUser> GetUserWithRolesByIdAsync(GetUserByIdRequest request, CallOptions options)
    {
        return ToAsyncUnaryCall(GetUserById(request.Id));
    }

    public override AsyncUnaryCall<GrpcUser> GetUserWithRolesByIdAsync(GetUserByIdRequest request,
        Metadata headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return ToAsyncUnaryCall(GetUserById(request.Id));
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

    private static GrpcUser GetUserById(long id)
    {
        var user = Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.UserNotFound),
                new Metadata { { "ErrorCode", ErrorCodes.UserNotFound.ToString() } });

        return Mapper.Map<GrpcUser>(user);
    }
}