using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;
using WebAPI.Repositories.Abstract;

namespace WebAPI.Repositories.Concrete;

public class WarehouseRepository : EfRepositoryBase<Warehouse, BaseDbContext>, IWarehouseRepository
{
    public WarehouseRepository(BaseDbContext context)
        : base(context) { }
}
