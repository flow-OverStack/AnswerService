using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnswerService.Api.Dto.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Results;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Helpers;
using Newtonsoft.Json;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests;

[Collection(nameof(AnswerServiceTests))]
[FunctionalTest]
public class AnswerServiceTests : SequentialFunctionalTest
{
    public AnswerServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaToken("testuser1", 1, [new RoleDto { Name = "User" }]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task PostAnswer_ValidBody_ReturnsCreated()
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
    public async Task PostAnswer_BodyTooShort_ReturnsBadRequest()
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
    public async Task DeleteAnswer_ExistingAnswer_ReturnsOk()
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
    public async Task DeleteAnswer_NonexistentUser_ReturnsNotFound()
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
    public async Task EditAnswer_ValidBody_ReturnsOk()
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
    public async Task EditAnswer_WrongOwner_ReturnsForbidden()
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
    public async Task AcceptAnswer_UnacceptedAnswer_ReturnsOk()
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
    public async Task AcceptAnswer_QuestionAlreadyHasAcceptedAnswer_ReturnsConflict()
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
    public async Task RevokeAnswerAcceptance_AcceptedAnswerOwnedByUser_ReturnsOk()
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
    public async Task RevokeAnswerAcceptance_NonexistentQuestion_ReturnsNotFound()
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
    public async Task DownvoteAnswer_NotYetVoted_ReturnsOk()
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
    public async Task DownvoteAnswer_AlreadyVoted_ReturnsConflict()
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
    public async Task UpvoteAnswer_NotYetVoted_ReturnsOk()
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
    public async Task UpvoteAnswer_TooLowReputation_ReturnsForbidden()
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
    public async Task RemoveVote_ExistingVote_ReturnsOk()
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
    public async Task RemoveVote_NoExistingVote_ReturnsNotFound()
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