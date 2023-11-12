using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;


namespace WebAPI.Repositories.Abstract;

public interface ICustomerWarehouseCostRepository : IAsyncRepository<CustomerWarehouseCost>, IRepository<CustomerWarehouseCost>
{
}
