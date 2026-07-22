using AnswerService.Domain.Dto.ExternalEntity;

namespace AnswerService.Tests.TestData;

internal static class QuestionDtoMother
{
    public static IQueryable<QuestionDto> GetQuestionDtos()
    {
        return new QuestionDto[]
        {
            new()
            {
                Id = 1,
                Title = "Test question 1",
                Body = "Test question body 1",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20))
            },
            new()
            {
                Id = 2,
                Title = "Test question 2",
                Body = "Test question body 2",
                UserId = 2,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20))
            },
            new()
            {
                Id = 3,
                Title = "Test question 3",
                Body = "Test question body 3",
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20))
            }
        }.AsQueryable();
    }
}
