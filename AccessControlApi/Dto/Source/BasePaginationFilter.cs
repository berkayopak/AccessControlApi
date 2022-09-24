namespace AccessControlApi.Dto.Source;

public class BasePaginationFilter
{
    public int Skip { get; }
    public int Take { get; }

    public BasePaginationFilter(int pageSize, int pageNumber)
    {
        pageSize = pageSize >= 1 ? pageSize : 1;
        pageNumber = pageNumber >= 1 ? pageNumber : 1;

        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
    }
}