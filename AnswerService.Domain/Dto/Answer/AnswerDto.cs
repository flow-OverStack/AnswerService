namespace AnswerService.Domain.Dto.Answer;

public record AnswerDto(long Id, string Body, long QuestionId, long UserId, bool IsAccepted);