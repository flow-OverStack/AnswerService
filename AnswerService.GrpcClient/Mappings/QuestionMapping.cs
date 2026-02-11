using AnswerService.Domain.Dto.ExternalEntity;
using AutoMapper;

namespace AnswerService.GrpcClient.Mappings;

public class QuestionMapping : Profile
{
    public QuestionMapping()
    {
        CreateMap<GrpcQuestion, QuestionDto>().ReverseMap();
    }
}