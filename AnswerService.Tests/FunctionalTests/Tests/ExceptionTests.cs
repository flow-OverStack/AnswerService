using System.Net;
using System.Net.Http.Headers;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Results;
using AnswerService.Tests.FunctionalTests.Base.Exception;
using AnswerService.Tests.FunctionalTests.Helper;
using Newtonsoft.Json;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests;

public class ExceptionTests : ExceptionFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteAnswer_ShouldBe_InternalServerError()
    {
        //Arrange
        const long answerId = 1;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AcceptAnswer_ShouldBe_InternalServerError()
    {
        //Arrange
        const long answerId = 4;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/accept", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RevokeAnswerAcceptance_ShouldBe_InternalServerError()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser3", 3, [
            new RoleDto { Name = "User" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const long answerId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/revoke-acceptance", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<AnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpvoteAnswer_ShouldBe_InternalServerError()
    {
        //Arrange
        const long answerId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/upvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteAnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DownvoteAnswer_ShouldBe_InternalServerError()
    {
        //Arrange
        const long answerId = 2;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/answer/{answerId}/downvote", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteAnswerDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RemoveVote_ShouldBe_InternalServerError()
    {
        //Arrange
        const long answerId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/answer/{answerId}/vote");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
    }
}