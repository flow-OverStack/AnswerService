using AnswerService.Domain.Dto.ExternalEntity;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Extensions;
using AnswerService.GraphQl.DataLoaders;
using HotChocolate.ApolloFederation.Types;

namespace AnswerService.GraphQl.Types.Extension;

public class QuestionType : ObjectType<QuestionDto>
{
    protected override void Configure(IObjectTypeDescriptor<QuestionDto> descriptor)
    {
        descriptor.Name("Question");
        descriptor.Description("The question type.");

        descriptor.ExtendServiceType();
        descriptor.Key(nameof(QuestionDto.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetQuestionById(default!, default!));

        descriptor.Field("answers")
            .Description("The answers of the question.")
            .ResolveWith<Resolvers>(x => x.GetQuestionAnswersAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<AnswerType>>>>();

        descriptor.Field(x => x.Title).Ignore();
        descriptor.Field(x => x.Body).Ignore();
        descriptor.Field(x => x.CreatedAt).Ignore();
        descriptor.Field(x => x.LastModifiedAt).Ignore();
        descriptor.Field(x => x.UserId).Ignore();
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Answer>> GetQuestionAnswersAsync([Parent] QuestionDto question,
            GroupQuestionAnswerDataLoader answerLoader,
            CancellationToken cancellationToken)
        {
            var answers = await answerLoader.LoadRequiredAsync(question.Id, cancellationToken);

            return answers;
        }

        public static QuestionDto GetQuestionById(long id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new QuestionDto { Id = id };
        }
    }
}