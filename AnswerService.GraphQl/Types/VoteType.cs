using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.GraphQl.DataLoaders;
using AnswerService.GraphQl.Types.Extension;

namespace AnswerService.GraphQl.Types;

public class VoteType : ObjectType<Vote>
{
    protected override void Configure(IObjectTypeDescriptor<Vote> descriptor)
    {
        descriptor.Name("AnswerVote");
        descriptor.Description("The answer vote type.");
        descriptor.Field(x => x.UserId).Description("The ID of the user that voted.");
        descriptor.Field(x => x.AnswerId).Description("The ID of the answer that was voted.");
        descriptor.Field(x => x.VoteTypeId).Description("The ID of the vote type.");
        descriptor.Field(x => x.VoteType).Description("The vote type.");
        descriptor.Field(x => x.Answer).Description("The answer that was voted.");

        descriptor.Field(x => x.Answer).ResolveWith<Resolvers>(x => x.GetAnswerAsync(default!, default!, default!));
        descriptor.Field(x => x.VoteType).ResolveWith<Resolvers>(x => x.GetVoteTypeAsync(default!, default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The voter.")
            .ResolveWith<Resolvers>(x => x.GetUserByVote(default!, default!))
            .Type<NonNullType<UserType>>();
    }

    private sealed class Resolvers
    {
        public async Task<Answer> GetAnswerAsync([Parent] Vote vote, AnswerDataLoader answerLoader,
            CancellationToken cancellationToken)
        {
            var answer = await answerLoader.LoadRequiredAsync(vote.AnswerId, cancellationToken);

            return answer;
        }

        public async Task<Domain.Entities.VoteType> GetVoteTypeAsync([Parent] Vote vote,
            VoteTypeDataLoader voteTypeLoader,
            CancellationToken cancellationToken)
        {
            var voteType = await voteTypeLoader.LoadRequiredAsync(vote.VoteTypeId, cancellationToken);

            return voteType;
        }

        public UserDto GetUserByVote([Parent] Vote vote, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new UserDto { Id = vote.UserId };
        }
    }
}