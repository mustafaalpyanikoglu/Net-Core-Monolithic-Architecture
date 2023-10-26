namespace WebAPI.Persistence.Dynamic;

public class DynamicQuery
{
    // The sorting criteria for the query.
    public IEnumerable<Sort>? Sort { get; set; }

    // The filtering criteria for the query.
    public Filter Filter { get; set; }

    public DynamicQuery() { }

    public DynamicQuery(IEnumerable<Sort>? sort, Filter filter)
    {
        Sort = sort;
        Filter = filter;
    }
}
