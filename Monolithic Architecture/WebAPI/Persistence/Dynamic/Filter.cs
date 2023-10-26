namespace WebAPI.Persistence.Dynamic;

public class Filter
{
    // The field on which to apply the filter.
    public string Field { get; set; }

    // The operator to use for the filter.
    public string Operator { get; set; }

    // The value to compare with when applying the filter.
    public string? Value { get; set; }

    // The logical operator to use for combining multiple filters.
    public string? Logic { get; set; }

    // Additional nested filters to be applied.
    public IEnumerable<Filter>? Filters { get; set; }

    public Filter()
    {

    }

    public Filter(string field, string @operator, string? value, string? logic, IEnumerable<Filter>? filters) : this()
    {
        Field = field;
        Operator = @operator;
        Value = value;
        Logic = logic;
        Filters = filters;
    }
}
