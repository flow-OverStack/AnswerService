using AnswerService.Domain.Dtos.ExternalEntity;
using AutoMapper;

namespace AnswerService.GrpcClient.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<GrpcUser, UserDto>().ReverseMap();
    }
}