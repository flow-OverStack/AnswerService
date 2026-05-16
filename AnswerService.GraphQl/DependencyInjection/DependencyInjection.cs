using AnswerService.Application.Settings;
using AnswerService.GraphQl.DataLoaders;
using AnswerService.GraphQl.ErrorFilters;
using AnswerService.GraphQl.Types;
using AnswerService.GraphQl.Types.Extension;
using AnswerService.GraphQl.Types.Sharable;
using GraphQL.Server.Ui.Voyager;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AnswerService.GraphQl.DependencyInjection;

public static class DependencyInjection
{
    private const string GraphQlEndpoint = "/graphql";
    private const string GraphQlVoyagerEndpoint = "/graphql-voyager";

    /// <summary>
    ///     Adds GraphQl services
    /// </summary>
    /// <param name="services"></param>
    public static void AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddQueryType<Queries>()
            .AddTypeExtension<UserType>()
            .AddTypeExtension<QuestionType>()
            .AddType<AnswerType>()
            .AddType<VoteType>()
            .AddType<VoteTypeType>()
            .AddTypeExtension<CollectionSegmentInfoType>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<AnswerDataLoader>()
            .AddDataLoader<GroupQuestionAnswerDataLoader>()
            .AddDataLoader<GroupVoteDataLoader>()
            .AddDataLoader<GroupUserVoteDataLoader>()
            .AddDataLoader<GroupUserAnswerDataLoader>()
            .AddDataLoader<GroupVoteTypeVoteDataLoader>()
            .AddDataLoader<VoteDataLoader>()
            .AddDataLoader<VoteTypeDataLoader>()
            .AddApolloFederation(FederationVersion.Federation23)
            .ModifyPagingOptions(opt =>
            {
                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateAsyncScope();
                var defaultSize = scope.ServiceProvider.GetRequiredService<IOptions<PaginationRules>>().Value
                    .DefaultPageSize;

                opt.DefaultPageSize = defaultSize;
                opt.IncludeTotalCount = true;
            })
            .AddDbContextCursorPagingProvider()
            .ModifyCostOptions(opt => opt.MaxFieldCost *= 3);
    }

    /// <summary>
    ///     Enables the use of GraphQl services
    /// </summary>
    /// <param name="app"></param>
    public static void UseGraphQl(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseGraphQLVoyager(GraphQlVoyagerEndpoint, new VoyagerOptions { GraphQLEndPoint = GraphQlEndpoint });

        if (app.Environment.IsDevelopment())
            app.MapGraphQL();
        else
            app.MapGraphQL().WithOptions(
                new GraphQLServerOptions
                {
                    Tool =
                    {
                        Enable = false
                    }
                });
    }
}