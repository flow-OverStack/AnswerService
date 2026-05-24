using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Extensions;
using AnswerService.GraphQl.DataLoaders;
using AnswerService.GraphQl.Types.Extension;
using HotChocolate.ApolloFederation.Types;

namespace AnswerService.GraphQl.Types;

public class AnswerType : ObjectType<Answer>
{
    protected override void Configure(IObjectTypeDescriptor<Answer> descriptor)
    {
        descriptor.Description("The answer type.");
        descriptor.Field(x => x.Id).Description("The ID of the answer.");
        descriptor.Field(x => x.Body).Description("The body of the answer.");
        descriptor.Field(x => x.CreatedAt).Description("The creation date of the answer.");
        descriptor.Field(x => x.LastModifiedAt).Description("The last update date of the answer");
        descriptor.Field(x => x.UserId).Description("The ID of the user who created the answer.");
        descriptor.Field(x => x.QuestionId).Description("The ID of the question that the answer belongs to.");
        descriptor.Field(x => x.IsAccepted).Description("Whether the answer is accepted.");
        descriptor.Field(x => x.Votes).Description("The votes of the answer.");
        descriptor.Field(x => x.Enabled).Ignore();

        descriptor.Field(x => x.Votes).ResolveWith<Resolvers>(x => x.GetVotesAsync(default!, default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The author of the answer.")
            .ResolveWith<Resolvers>(x => x.GetUserByAnswerAsync(default!, default!))
            .Type<NonNullType<UserType>>();

        descriptor.Field("question") // Field for question from QuestionService
            .Description("The question that the answer belongs to.")
            .ResolveWith<Resolvers>(x => x.GetQuestionByAnswerAsync(default!, default!))
            .Type<NonNullType<QuestionType>>();

        descriptor.Field("reputation")
            .Type<NonNullType<IntType>>()
            .Description("The reputation of the answer.")
            .ResolveWith<Resolvers>(x => x.CalculateReputationAsync(default!, default!, default!, default!));

        descriptor.Key(nameof(Answer.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetAnswerByIdAsync(default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Vote>> GetVotesAsync([Parent] Answer answer, GroupVoteDataLoader voteLoader,
            CancellationToken cancellationToken)
        {
            var votes = await voteLoader.LoadRequiredAsync(answer.Id, cancellationToken);

            return votes;
        }

        public static async Task<Answer> GetAnswerByIdAsync(long id, AnswerDataLoader answerLoader,
            CancellationToken cancellationToken)
        {
            var answer = await answerLoader.LoadRequiredAsync(id, cancellationToken);

            return answer;
        }

        public async Task<UserDto> GetUserByAnswerAsync([Parent] Answer answer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return new UserDto { Id = answer.UserId };
        }

        public async Task<QuestionDto> GetQuestionByAnswerAsync([Parent] Answer answer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return new QuestionDto { Id = answer.QuestionId };
        }

        public async Task<int> CalculateReputationAsync([Parent] Answer answer, GroupVoteDataLoader voteLoader,
            VoteTypeDataLoader voteTypeLoader, CancellationToken cancellationToken)
        {
            var votes = await voteLoader.LoadRequiredAsync(answer.Id, cancellationToken);
            var voteTypes =
                await voteTypeLoader.LoadRequiredAsync(votes.Select(x => x.VoteTypeId).ToArray(), cancellationToken);

            var sum = voteTypes.Sum(x => x.ReputationChange);
            return sum;
        }
    }
}