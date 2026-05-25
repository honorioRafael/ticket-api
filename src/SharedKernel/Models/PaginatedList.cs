namespace SharedKernel.Models;

public record PaginatedList<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
