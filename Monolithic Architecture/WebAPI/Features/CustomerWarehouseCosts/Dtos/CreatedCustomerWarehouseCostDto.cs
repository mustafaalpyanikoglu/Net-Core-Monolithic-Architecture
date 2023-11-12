using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Dtos;

public class CreatedCustomerWarehouseCostDto : IDto
{
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
