using WebAPI.Models.Concrete;

namespace BusinessLayer.Services.UserService;

public interface IUserService
{
    Task<User?> GetByEmail(string email);
    Task<User> GetById(int id);
    Task<User> Update(User user);
    Task<string> UpdateImage(IFormFile file, string imageUrl);
    Task ActivateTheUser(int userId);
    Task<string> UpdateCV(IFormFile file, string cvUrl);
}
