using System.Net;
using System.Net.Http.Json;
using AnswerService.Application.Handlers.Get.Vote;
using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Tests.FunctionalTests.Base;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using AnswerService.Tests.FunctionalTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class CacheGetServicesTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    // Only functional tests are provided for cache services' success scenarios.
    // This is because cache data mirrors the database, and manually copying test DB data into multiple cache keys/values is impractical and confusing.
    // In functional tests, data is automatically copied from the DB to the cache as needed, following all key/value rules.

    [Fact]
    public async Task GetAnswerById_CacheHit_ReturnsOk()
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
        Assert.NotNull(result!.Data.Answer.Votes);
    }

    [Fact]
    public async Task GetAnswerById_NonexistentAnswerCached_ReturnsNull()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAnswerByIdQuery(0) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Answer);
    }

    [Fact]
    public async Task GetAnswerById_CorruptedCacheEntry_ReturnsOk()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDatabase>();
        var requestBody = new { query = GraphQlHelper.RequestAnswerByIdQuery(2) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // Simulate a wrong entry in the cache
        await cache.StringSetAsync("vote:2,3", "Wrong data");
        // 2nd request fetches data from the cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Answer);
        Assert.All(result.Data.Answer.Votes.Select(x => x.VoteType), Assert.NotNull);
    }

    [Fact]
    public async Task GetGroupedById_AnswerVotesCachedAsNull_ReturnsEmpty()
    {
        //Arrange
        const long answerId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVoteCacheRepository>();
        // Inner service is not in the DI
        var inner = ActivatorUtilities.CreateInstance<GetAnswersVotesHandler>(scope.ServiceProvider);
        var fetch = async (IEnumerable<long> idsToFetch, CancellationToken ct) =>
            (await inner.Handle(new GetAnswersVotesQuery(idsToFetch), ct)).Data ?? [];

        //Act
        // The first call marks the user as null in the cache
        await repository.GetAnswersVotesAsync([answerId], fetch);
        // The second call fetches the null entry from the cache
        var result = await repository.GetAnswersVotesAsync([answerId], fetch);

        //Assert
        Assert.Empty(result);
    }
}