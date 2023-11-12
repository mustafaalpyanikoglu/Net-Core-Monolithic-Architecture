using WebAPI.Application.Rules;
using static BusinessLayer.Features.Users.Constants.UserMessages;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using System.Security.Claims;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using WebAPI.Security.Constants;

namespace BusinessLayer.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userDal;

    public UserBusinessRules(IUserRepository userDal)
    {
        _userDal = userDal;
    }

    public async Task UserIdMustBeAvailable(int id)
    {
        // Checks if a user with the provided ID exists, and throws an exception if it doesn't.
        User? result = await _userDal.GetAsync(t => t.Id == id);
        if (result == null) throw new BusinessException(UserNotFound);
    }

    public async Task ShouldNotHaveUserCart(int userId)
    {
        // Checks if a user with the provided ID has a user cart, and throws an exception if they do.
        User? result = await _userDal.GetAsync(t => t.Id == userId);
        if (result != null) throw new BusinessException(ThisUserHasAUserCart);
    }

    public Task UserMustBeLoggedIn(List<Claim>? claims)
    {
        if (claims is null || claims.Count <= 0) throw new BusinessException(UserIsNotLoggedIn);
        return Task.CompletedTask;
    }

    public async Task ShouldNotHavePuser(int userId)
    {
        // Checks if a user with the provided ID has a purse, and throws an exception if they do.
        User? result = await _userDal.GetAsync(t => t.Id == userId);
        if (result != null) throw new BusinessException(ThisUserHasAPurse);
    }

    public async Task UserEmailMustBeAvailable(string email)
    {
        // Checks if a user with the provided email exists, and throws an exception if they don't.
        User? result = await _userDal.GetAsync(t => t.Email == email);
        if (result == null) throw new BusinessException(UserNotFound);
    }

    public async Task UserEmailMustNotExist(string email)
    {
        // Checks if a user with the provided email doesn't exist, and throws an exception if they do.
        User? result = await _userDal.GetAsync(t => t.Email == email);
        if (result != null) throw new BusinessException(UserEmailAvaliable);
    }

    public async Task UserMustBeAvailable()
    {
        // Checks if there is at least one user available, and throws an exception if there are none.
        List<User>? results = _userDal.GetAll();
        if (results.Count <= 0) throw new BusinessException(UserNotFound);
    }

    public Task UserShouldBeExist(User? user)
    {
        // Checks if a user exists, and throws an exception if they don't.
        if (user is null) throw new BusinessException(UserDontExists);
        return Task.CompletedTask;
    }

    public Task UserPasswordShouldBeMatch(User user, string password)
    {
        // Checks if the provided password matches the user's password, and throws an exception if they don't match.
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(PasswordDontMatch);
        return Task.CompletedTask;
    }

}