using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnswerService.Api.Dto.Answer;
using AnswerService.Application.Resources;
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

    [Fact]
    [Trait("Category", "Functional")]
    public async Task PostAnswer_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new PostAnswerDto(3, "Too short body");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/answer", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidAnswerBody, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task DeleteAnswer_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 1;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task DeleteAnswer_ShouldBe_NotFound()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("nonexistentuser", 0, [new RoleDto { Name = "User" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const long answerId = 1;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task EditAnswer_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 1;
        var dto = new EditAnswerDto("Edited Body Edited Body Edited Body ");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/answer/{answerId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task EditAnswer_ShouldBe_Forbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser2", 2, [new RoleDto { Name = "User" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        const long answerId = 1;
        var dto = new EditAnswerDto("Edited Body Edited Body Edited Body ");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/answer/{answerId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task AcceptAnswer_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 4;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/accept", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task AcceptAnswer_ShouldBe_Conflict()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser3", 3, [new RoleDto { Name = "User" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        const long answerId = 1;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/accept", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionAlreadyHasAcceptedAnswer, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RevokeAnswerAcceptance_ShouldBe_Ok()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser2", 2, [new RoleDto { Name = "User" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const long answerId = 3;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/revoke-acceptance", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RevokeAnswerAcceptance_ShouldBe_NotFound()
    {
        //Arrange
        const long answerId = 5;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/revoke-acceptance", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task DownvoteAnswer_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task DownvoteAnswer_ShouldBe_Conflict()
    {
        //Arrange
        const long answerId = 4;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task UpvoteAnswer_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task UpvoteAnswer_ShouldBe_Forbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser2", 2, [new RoleDto { Name = "User" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const long answerId = 3;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RemoveVote_ShouldBe_Ok()
    {
        //Arrange
        const long answerId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RemoveVote_ShouldBe_NotFound()
    {
        //Arrange
        const long answerId = 2;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}