namespace WebAPI.Models.Dtos.WarehousesDtos;

public class CreatedWarehouseDto : IDto
{
    public int Capacity { get; set; }
    public double SetupCost { get; set; }
}
