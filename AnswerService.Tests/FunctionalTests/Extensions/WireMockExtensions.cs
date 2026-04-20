using WireMock.Server;

namespace AnswerService.Tests.FunctionalTests.Extensions;

internal static class WireMockExtensions
{
    public static void StopServer(this WireMockServer server)
    {
        server.Stop();
    }
}