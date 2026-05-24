using AnswerService.Domain.Entities;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlCursorPaginatedResponse<GraphQlAnswer> Answers { get; set; }
    public GraphQlOffsetPaginatedResponse<Vote> AnswerVotes { get; set; }
    public GraphQlOffsetPaginatedResponse<VoteType> AnswerVoteTypes { get; set; }
}