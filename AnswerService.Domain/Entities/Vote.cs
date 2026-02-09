namespace AnswerService.Domain.Entities;

public class Vote
{
    public long UserId { get; set; }

    public long AnswerId { get; set; }
    public Answer Answer { get; set; }

    public long VoteTypeId { get; set; }
    public VoteType VoteType { get; set; }
}