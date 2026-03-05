namespace AnswerService.Application.Enum;

public enum ErrorCodes
{
    //Data: 1-10,
    //User: 11-20,
    //Question: 21-30,
    //Answer: 31-40,
    //Authorization: 41-50

    InvalidProperty = 1,

    UserNotFound = 11,

    QuestionNotFound = 21,

    AnswerNotFound = 31,
    AnswerAlreadyExists = 32,

    OperationForbidden = 41
}