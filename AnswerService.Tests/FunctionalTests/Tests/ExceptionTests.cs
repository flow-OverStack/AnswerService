using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dtos.Answer;
using AnswerService.Domain.Dtos.ExternalEntity;
using AnswerService.Domain.Results;
using AnswerService.Tests.FunctionalTests.Base.Exception;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using AnswerService.Tests.FunctionalTests.Helpers;
using AnswerService.Tests.Traits;
using Newtonsoft.Json;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ExceptionTests : ExceptionFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task DeleteAnswer_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task AcceptAnswer_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task RevokeAnswerAcceptance_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task UpvoteAnswer_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task DownvoteAnswer_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task RemoveVote_TransactionCommitFails_ReturnsInternalServerError()
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

    [Fact]
    public async Task GetAnswerById_CacheReadFailure_ReturnsOk()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAnswerByIdQuery(2) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Answer);
        Assert.NotNull(result.Data.Answer.Votes);
    }
}