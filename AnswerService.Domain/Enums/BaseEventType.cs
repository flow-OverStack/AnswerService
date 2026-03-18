namespace AnswerService.Domain.Enums;

public enum BaseEventType
{
    EntityAccepted,

    EntityUpvoted,
    EntityDownvoted,
    EntityDeleted,

    EntityVoteRemoved,
    EntityAcceptanceRevoked
}