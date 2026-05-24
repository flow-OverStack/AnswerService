using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AnswerService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class GraphQlExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBaseRepository<Answer>>();
            services.AddScoped<IBaseRepository<Answer>>(_ => null!); //Passing null to cause NullReferenceException
        });
    }
}