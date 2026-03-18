using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Interfaces.Provider;
using AutoMapper;
using Grpc.Core;

namespace AnswerService.GrpcClient.Providers;

public class UserProvider(UserService.UserServiceClient client, IMapper mapper) : IEntityProvider<UserDto>
{
    public async Task<UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await client.GetUserWithRolesByIdAsync(new GetUserByIdRequest { Id = id },
                cancellationToken: cancellationToken);
            return mapper.Map<UserDto>(user);
        }
        catch (RpcException e) when (e.Status.Detail == ErrorMessage.UserNotFound)
        {
            return null;
        }
    }
}