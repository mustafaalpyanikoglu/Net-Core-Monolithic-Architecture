using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;
using WebAPI.Repositories.Abstract;

namespace WebAPI.Repositories.Concrete;

public class EmailAuthenticatorRepository : EfRepositoryBase<EmailAuthenticator, BaseDbContext>, IEmailAuthenticatorRepository
{
    public EmailAuthenticatorRepository(BaseDbContext context)
        : base(context) { }
}

