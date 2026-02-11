using AnswerService.Domain.Dto.ExternalEntity;
using AutoMapper;

namespace AnswerService.GrpcClient.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<GrpcRole, RoleDto>().ReverseMap();
    }
}