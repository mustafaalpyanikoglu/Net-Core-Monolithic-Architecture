namespace WebAPI.Models.Dtos.CustomerWarehouseCostsDtos;

public class CreatedCustomerWarehouseCostDto : IDto
{
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
