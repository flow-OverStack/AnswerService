namespace AnswerService.Domain.Dto.ExternalEntity;

public class QuestionDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}