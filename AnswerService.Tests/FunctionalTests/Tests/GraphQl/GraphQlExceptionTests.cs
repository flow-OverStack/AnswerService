using System.Net;
using System.Net.Http.Json;
using AnswerService.Application.Resources;
using AnswerService.Tests.FunctionalTests.Base.Exception.GraphQl;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using AnswerService.Tests.FunctionalTests.Helper;
using Newtonsoft.Json;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlExceptionTests(GraphQlExceptionFunctionalTestWebAppFactory factory)
    : GraphQlExceptionFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ResolverThrowsException_ReturnsInternalServerError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result.Errors, x => x.Message.StartsWith(ErrorMessage.InternalServerError));
    }
}