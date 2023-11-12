using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IWarehouseRepository : IAsyncRepository<Warehouse>, IRepository<Warehouse>
{
}
