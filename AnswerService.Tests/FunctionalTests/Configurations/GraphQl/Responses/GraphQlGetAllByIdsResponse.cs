using AnswerService.Domain.Entities;
using AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetAllByIdsResponse
{
    public GetAllByIdsData Data { get; set; }
}

internal class GetAllByIdsData
{
    public GraphQlAnswer? Answer { get; set; }
    public Vote? AnswerVote { get; set; }
    public VoteType? AnswerVoteType { get; set; }
}