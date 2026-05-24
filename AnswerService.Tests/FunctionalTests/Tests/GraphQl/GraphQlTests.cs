using System.Net;
using System.Net.Http.Json;
using AnswerService.Application.Resources;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using AnswerService.Tests.FunctionalTests.Helper;
using Newtonsoft.Json;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotEmpty(result!.Data.Answers.Edges);
        Assert.NotEqual(0, result.Data.Answers.TotalCount);
        Assert.All(result.Data.Answers.Edges, x =>
        {
            Assert.NotNull(x.Cursor);
            Assert.NotNull(x.Node);
        });

        Assert.NotEmpty(result.Data.AnswerVotes.Items);
        Assert.NotEqual(0, result.Data.AnswerVotes.TotalCount);

        Assert.NotEmpty(result.Data.AnswerVoteTypes.Items);
        Assert.NotEqual(0, result.Data.AnswerVoteTypes.TotalCount);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_InvalidPaginationError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestWithInvalidPaginationQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, result!.Errors.Count);
        Assert.All(result.Errors, x => Assert.StartsWith(ErrorMessage.InvalidPagination, x.Message));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(1, 1, 3, 1) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Answer);
        Assert.NotNull(result.Data.AnswerVote);
        Assert.NotNull(result.Data.AnswerVoteType);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Null()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(0, 0, 0, 0) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Answer);
        Assert.Null(result.Data.AnswerVote);
        Assert.Null(result.Data.AnswerVoteType);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestWithWrongArgument_ShouldBe_Error()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestWithWrongArgument };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Single(result!.Errors);
        Assert.NotNull(result.Errors[0].Extensions?.Code);
    }
}