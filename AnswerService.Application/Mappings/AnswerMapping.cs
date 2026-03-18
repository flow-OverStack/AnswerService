using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Entities;
using AutoMapper;

namespace AnswerService.Application.Mappings;

public class AnswerMapping : Profile
{
    public AnswerMapping()
    {
        CreateMap<Answer, AnswerDto>().ReverseMap();
        CreateMap<Answer, VoteAnswerDto>().ReverseMap();
        CreateMap<Answer, PostAnswerCommand>().ReverseMap();
        CreateMap<Answer, EditAnswerCommand>().ReverseMap();
    }
}