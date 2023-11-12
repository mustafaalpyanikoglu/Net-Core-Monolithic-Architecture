using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IOtpAuthenticatorRepository : IAsyncRepository<OtpAuthenticator>, IRepository<OtpAuthenticator> { }
