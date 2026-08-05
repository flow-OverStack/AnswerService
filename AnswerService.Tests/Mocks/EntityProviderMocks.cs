using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Tests.TestData;
using Moq;

namespace AnswerService.Tests.Mocks;

internal static class EntityProviderMocks
{
    public static IMock<IEntityProvider<QuestionDto>> GetMockQuestionProvider()
    {
        var mockProvider = new Mock<IEntityProvider<QuestionDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long questionId, CancellationToken _) =>
                QuestionDtoMother.GetQuestionDtos().FirstOrDefault(x => x.Id == questionId));
        return mockProvider;
    }

    public static IMock<IEntityProvider<UserDto>> GetMockUserProvider()
    {
        var mockProvider = new Mock<IEntityProvider<UserDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long userId, CancellationToken _) =>
                UserDtoMother.GetUserDtos().FirstOrDefault(x => x.Id == userId));
        return mockProvider;
    }
}
