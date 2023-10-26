namespace WebAPI.Persistence.Dynamic;

public class Sort
{
    // The field on which to apply the sorting.
    public string Field { get; set; }

    // The sort direction (ascending or descending).
    public string Dir { get; set; }

    public Sort()
    {

    }

    public Sort(string field, string dir)
    {
        Field = field;
        Dir = dir;
    }
}
