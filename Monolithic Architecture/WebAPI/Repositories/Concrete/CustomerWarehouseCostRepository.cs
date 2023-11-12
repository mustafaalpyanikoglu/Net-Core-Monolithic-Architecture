using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;
using WebAPI.Repositories.Abstract;

namespace WebAPI.Repositories.Concrete;

public class CustomerWarehouseCostRepository : EfRepositoryBase<CustomerWarehouseCost, BaseDbContext>, ICustomerWarehouseCostRepository
{
    public CustomerWarehouseCostRepository(BaseDbContext context)
        : base(context) { }
}
