namespace WebAPI.Models.Dtos.CustomerDtos;

public class CreatedCustomerDto : IDto
{
    public int UserID { get; set; }
    public int Demand { get; set; }
}
