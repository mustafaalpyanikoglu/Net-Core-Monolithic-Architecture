using WebAPI.Persistence.Dynamic;

namespace WebAPI.Application.Requests;

public class QueryModel
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }
}
