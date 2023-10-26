using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IUserRepository : IAsyncRepository<User>, IRepository<User>
{
}
