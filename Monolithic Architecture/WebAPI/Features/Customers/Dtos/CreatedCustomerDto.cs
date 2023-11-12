using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Customers.Dtos;

public class CreatedCustomerDto : IDto
{
    public int UserID { get; set; }
    public int Demand { get; set; }
}
