namespace WebAPI.Models.Dtos.WarehousesDtos;

public class UpdatedWarehouseDto : IDto
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
