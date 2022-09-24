namespace AccessControlApi.Dto.Response;

public class BasePaginationResponse<T>
{
    public T? Data { get; }
    public int TotalItemCount { get; }
    public int TotalPageCount { get; }
    public int CurrentPageNumber { get; }
    public int PageSize { get; }

    public BasePaginationResponse(T? data, int totalItemCount, int totalPageCount, int currentPageNumber, int pageSize)
    {
        Data = data;
        TotalItemCount = totalItemCount;
        TotalPageCount = totalPageCount;
        CurrentPageNumber = currentPageNumber;
        PageSize = pageSize;
    }
}