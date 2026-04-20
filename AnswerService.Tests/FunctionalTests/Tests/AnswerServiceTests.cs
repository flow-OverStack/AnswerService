using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnswerService.Api.Dto.Answer;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Results;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Helper;
using Newtonsoft.Json;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests;

[Collection(nameof(AnswerServiceTests))]
public class AnswerServiceTests : SequentialFunctionalTest
{
    public AnswerServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [new RoleDto { Name = "User" }]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task PostAnswer_ShouldBe_Created()
    {
        //Arrange
        var dto = new PostAnswerDto(3, "Test Body Test Body Test Body ");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/answer", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }
}