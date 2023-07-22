namespace DurgerKing.Dtos;

public class PaginatedList<T>
{
    public PaginatedList() { }
    public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageLimit)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageLimit);
        Items = items;
    }

    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public IEnumerable<T> Items { get; private set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}