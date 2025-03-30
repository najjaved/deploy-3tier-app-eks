namespace Basic3Tier.Infrastructure.Models;

public class PaginationResult<TRequest> where TRequest : CommonDtoRequest
{
    public List<TRequest> Data { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages => PageSize == 0 ? 1 : (int)Math.Ceiling((decimal)TotalRecords / PageSize);

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < TotalPages;
}