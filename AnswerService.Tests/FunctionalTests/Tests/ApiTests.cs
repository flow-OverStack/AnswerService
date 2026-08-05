using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using AnswerService.Api.Dto.Answer;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Helpers;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task RequestForbiddenResource_InvalidClaims_ReturnsForbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken("testuser2", 2, []);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await HttpClient.PutAsync("/api/v1.0/answer/1", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Fact]
    public async Task PostAnswer_MissingAuthToken_ReturnsUnauthorized()
    {
        //Arrange
        var dto = new PostAnswerDto(3, "Test Body Test Body Test Body ");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/answer", dto);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task RequestSwagger_DefaultRequest_ReturnsOk()
    {
        //Arrange
        const string swaggerUrl = "/swagger/v1/swagger.json";

        //Act
        var response = await HttpClient.GetAsync(swaggerUrl);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}