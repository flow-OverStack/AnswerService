using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Interfaces.Provider;
using Moq;

namespace AnswerService.Tests.Configurations;

internal static class MockEntityProvidersGetters
{
    public const int MinReputation = 1;

    public static IMock<IEntityProvider<QuestionDto>> GetMockQuestionProvider()
    {
        var mockProvider = new Mock<IEntityProvider<QuestionDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long questionId, CancellationToken _) =>
                GetQuestionDtos().FirstOrDefault(x => x.Id == questionId));
        return mockProvider;
    }

    public static IMock<IEntityProvider<UserDto>> GetMockUserProvider()
    {
        var mockProvider = new Mock<IEntityProvider<UserDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long userId, CancellationToken _) => GetUserDtos().FirstOrDefault(x => x.Id == userId));
        return mockProvider;
    }

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

    public static IQueryable<UserDto> GetUserDtos()
    {
        return new UserDto[]
        {
            new()
            {
                Id = 1,
                IdentityId = Guid.NewGuid(),
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleUser(), GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                IdentityId = Guid.NewGuid(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                Roles = [GetRoleUser()]
            },
            new()
            {
                Id = 3,
                IdentityId = Guid.NewGuid(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleModer()]
            },
            new()
            {
                Id = 4,
                IdentityId = Guid.NewGuid(),
                Username = "testuser4",
                Email = "TestUser4@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 15,
                Roles = [GetRoleModer()]
            }
        }.AsQueryable();
    }

    // Get entity dtos methods

    private static RoleDto GetRoleUser()
    {
        return new RoleDto
        {
            Id = 1,
            Name = "User"
        };
    }

    private static RoleDto GetRoleAdmin()
    {
        return new RoleDto
        {
            Id = 2,
            Name = "Admin"
        };
    }

    private static RoleDto GetRoleModer()
    {
        return new RoleDto
        {
            Id = 3,
            Name = "Moderator"
        };
    }
}