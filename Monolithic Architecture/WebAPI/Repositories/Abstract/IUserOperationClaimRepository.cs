using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IUserOperationClaimRepository : IAsyncRepository<UserOperationClaim>, IRepository<UserOperationClaim> { }
