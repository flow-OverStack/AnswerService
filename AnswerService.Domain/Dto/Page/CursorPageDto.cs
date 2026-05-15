namespace AnswerService.Domain.Dto.Page;

public record CursorPageDto(int? First, string? After, string? Before, int? Last, IEnumerable<OrderDto>? Order);