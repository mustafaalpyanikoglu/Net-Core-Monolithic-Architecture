using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Warehouses.Dtos;

public class UpdatedWarehouseDto : IDto
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
