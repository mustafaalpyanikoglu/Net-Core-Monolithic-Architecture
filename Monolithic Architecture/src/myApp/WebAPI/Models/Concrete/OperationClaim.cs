namespace WebAPI.Models.Concrete;

public class OperationClaim : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public OperationClaim()
    {

    }

    public OperationClaim(int id, string name, string description) : this()
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
