using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;

namespace WebAPI.Repositories.Abstract;

public interface IEmailAuthenticatorRepository : IAsyncRepository<EmailAuthenticator>, IRepository<EmailAuthenticator> { }
