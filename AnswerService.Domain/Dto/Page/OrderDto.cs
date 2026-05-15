using AnswerService.Domain.Enums;

namespace AnswerService.Domain.Dto.Page;

public record OrderDto(string Field, SortDirection Direction);