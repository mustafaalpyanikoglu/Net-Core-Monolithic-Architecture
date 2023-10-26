using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IOperationClaimRepository : IAsyncRepository<OperationClaim>, IRepository<OperationClaim> { }
