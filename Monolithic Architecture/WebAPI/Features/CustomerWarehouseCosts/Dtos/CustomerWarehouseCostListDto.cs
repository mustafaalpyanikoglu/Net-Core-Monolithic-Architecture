using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Dtos;

public class CustomerWarehouseCostListDto : IDto
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
