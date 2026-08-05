using AnswerService.Application.Mappings;
using AutoMapper;

namespace AnswerService.Tests.UnitTests.Fixtures;

internal static class MapperFixture
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(typeof(AnswerMapping)));
        return mockMapper.CreateMapper();
    }
}