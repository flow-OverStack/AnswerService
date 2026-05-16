using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Extensions;
using AnswerService.GraphQl.DataLoaders;
using HotChocolate.ApolloFederation.Types;

namespace AnswerService.GraphQl.Types.Extension;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        descriptor.Description("The user type.");
        descriptor.ExtendServiceType();
        descriptor.Key(nameof(UserDto.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetUserById(default!, default!));

        descriptor.Field("answers")
            .Description("The answers of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserAnswersAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<AnswerType>>>>();

        descriptor.Field("answerVotes")
            .Description("The answer votes of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserVotesAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<VoteType>>>>();

        descriptor.Field(x => x.IdentityId).Ignore();
        descriptor.Field(x => x.Username).Ignore();
        descriptor.Field(x => x.Email).Ignore();
        descriptor.Field(x => x.LastLoginAt).Ignore();
        descriptor.Field(x => x.Reputation).Ignore();
        descriptor.Field(x => x.Roles).Ignore();
        descriptor.Field(x => x.CreatedAt).Ignore();
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Answer>> GetUserAnswersAsync([Parent] UserDto user,
            GroupUserAnswerDataLoader answerLoader, CancellationToken cancellationToken)
        {
            var answers = await answerLoader.LoadRequiredAsync(user.Id, cancellationToken);

            return answers;
        }

        public async Task<IEnumerable<Vote>> GetUserVotesAsync([Parent] UserDto user,
            GroupUserVoteDataLoader voteLoader, CancellationToken cancellationToken)
        {
            var votes = await voteLoader.LoadRequiredAsync(user.Id, cancellationToken);

            return votes;
        }

        public static UserDto GetUserById(long id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new UserDto { Id = id };
        }
    }
}