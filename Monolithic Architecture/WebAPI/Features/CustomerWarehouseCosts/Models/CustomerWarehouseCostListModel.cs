using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using WebAPI.Persistence.Paging;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Models;

public class CustomerWarehouseCostListModel : BasePageableModel
{
    public IList<CustomerWarehouseCostListDto> Items { get; set; }
}
