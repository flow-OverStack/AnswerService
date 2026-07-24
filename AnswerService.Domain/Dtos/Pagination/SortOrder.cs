using AnswerService.Domain.Enums;

namespace AnswerService.Domain.Dtos.Pagination;

public record SortOrder(string Field, SortDirection Direction);