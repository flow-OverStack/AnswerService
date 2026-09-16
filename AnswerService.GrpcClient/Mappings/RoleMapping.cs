using AnswerService.Domain.Dtos.ExternalEntity;
using AutoMapper;

namespace AnswerService.GrpcClient.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<GrpcRole, RoleDto>().ReverseMap();
    }
}