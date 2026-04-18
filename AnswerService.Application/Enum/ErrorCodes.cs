namespace AnswerService.Application.Enum;

public enum ErrorCodes
{
    //Data: 1-10,
    //User: 11-20,
    //Question: 21-30,
    //Answer: 31-40,
    //Authorization: 41-50,
    //Vote: 51-60

    InvalidProperty = 1,

    UserNotFound = 11,

    QuestionNotFound = 21,

    AnswerNotFound = 31,
    AnswerAlreadyExists = 32,
    AnswerAlreadyAccepted = 33,
    AnswerNotAccepted = 34,
    QuestionAlreadyHasAcceptedAnswer = 35,

    OperationForbidden = 41,

    VoteAlreadyGiven = 51,
    VoteNotFound = 52,
    VotesNotFound = 53,
    VoteTypeNotFound = 54,
    VoteTypesNotFound = 55,
    CannotVoteForOwnPost = 56
}