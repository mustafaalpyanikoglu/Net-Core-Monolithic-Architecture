using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;
using WebAPI.Repositories.Abstract;

namespace WebAPI.Repositories.Concrete;

public class OtpAuthenticatorRepository : EfRepositoryBase<OtpAuthenticator, BaseDbContext>, IOtpAuthenticatorRepository
{
    public OtpAuthenticatorRepository(BaseDbContext context)
        : base(context) { }
}
