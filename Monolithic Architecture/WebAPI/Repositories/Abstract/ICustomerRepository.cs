using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface ICustomerRepository : IAsyncRepository<Customer>, IRepository<Customer>
{
}
