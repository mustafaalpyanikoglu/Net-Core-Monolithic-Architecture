namespace WebAPI.Models.Concrete;

public class Customer : Entity
{
    public int UserID { get; set; }
    public int Demand { get; set; }

    public virtual User User { get; set; }
    public ICollection<CustomerWarehouseCost> CustomerWarehouseCosts { get; set; }

    public Customer()
    {

    }

    public Customer(int id, int userId, int demand) : this()
    {
        Id = id;
        UserID = userId;
        Demand = demand;
    }
}
