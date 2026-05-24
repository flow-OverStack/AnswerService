using AnswerService.Domain.Entities;

namespace AnswerService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

internal class GraphQlAnswer : Answer
{
    public int Reputation { get; set; }
}