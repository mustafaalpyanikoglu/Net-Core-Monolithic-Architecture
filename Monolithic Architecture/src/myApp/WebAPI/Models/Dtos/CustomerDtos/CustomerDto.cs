namespace WebAPI.Models.Dtos.CustomerDtos;

public class CustomerDto : IDto
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int Demand { get; set; }
}
