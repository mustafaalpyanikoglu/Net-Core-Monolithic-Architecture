using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Warehouses.Dtos;

public class CreatedWarehouseDto : IDto
{
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
