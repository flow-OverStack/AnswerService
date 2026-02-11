using AnswerService.Domain.Dto.ExternalEntity;
using AutoMapper;

namespace AnswerService.GrpcClient.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<GrpcUser, UserDto>().ReverseMap();
    }
}