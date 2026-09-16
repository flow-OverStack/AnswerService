using System.Net;
using System.Net.Http.Json;
using AnswerService.Application.Resources;
using AnswerService.Tests.FunctionalTests.Base.Exception.GraphQl;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using AnswerService.Tests.FunctionalTests.Helpers;
using Newtonsoft.Json;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl;

[FunctionalTest]
public class GraphQlExceptionTests(GraphQlExceptionFunctionalTestWebAppFactory factory)
    : GraphQlExceptionFunctionalTest(factory)
{
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