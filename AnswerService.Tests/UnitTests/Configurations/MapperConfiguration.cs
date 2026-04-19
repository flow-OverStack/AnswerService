using AnswerService.Application.Mappings;
using AutoMapper;

namespace AnswerService.Tests.UnitTests.Configurations;

internal static class MapperConfiguration
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(typeof(AnswerMapping)));
        return mockMapper.CreateMapper();
    }
}