using AnswerService.Domain.Interfaces.Entity;

namespace AnswerService.Domain.Entities;

public class Answer : IEntityId<long>, IAuditable
{
    public string Body { get; set; }
    public long UserId { get; set; }
    public long QuestionId { get; set; }
    public bool IsAccepted { get; set; }
    public List<Vote> Votes { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public long Id { get; set; }
}