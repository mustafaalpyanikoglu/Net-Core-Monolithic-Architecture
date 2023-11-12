using BusinessLayer.Features.Customers.Dtos;
using WebAPI.Persistence.Paging;

namespace BusinessLayer.Features.Customers.Models;

public class CustomerListModel : BasePageableModel
{
    public IList<CustomerListDto> Items { get; set; }
}
