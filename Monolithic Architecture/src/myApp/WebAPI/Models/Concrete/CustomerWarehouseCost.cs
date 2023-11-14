namespace WebAPI.Models.Concrete;

public class CustomerWarehouseCost : Entity
{
    public int CustomerId { get; set; }
    public int WarehouseID { get; set; }
    public double Cost { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual Warehouse Warehouse { get; set; }

    public CustomerWarehouseCost()
    {

    }

    public CustomerWarehouseCost(int id, int customerId, int warehouseId, double cost) : this()
    {
        Id = id;
        CustomerId = customerId;
        WarehouseID = warehouseId;
        Cost = cost;
    }
}
