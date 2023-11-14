namespace WebAPI.Models.Dtos.CustomerDtos;

public class CustomerListDto : IDto
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int Demand { get; set; }
}
