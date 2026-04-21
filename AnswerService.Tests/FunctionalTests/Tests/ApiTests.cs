using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using AnswerService.Api.Dto.Answer;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Helper;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Forbidden_When_ClaimsNotValid()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task PostAnswer_ShouldBe_Unauthorized()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestSwagger_ShouldBe_Success()
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