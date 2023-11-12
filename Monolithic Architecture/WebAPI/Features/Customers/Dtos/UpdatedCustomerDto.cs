using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Customers.Dtos;

public class UpdatedCustomerDto : IDto
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int Demand { get; set; }
}
