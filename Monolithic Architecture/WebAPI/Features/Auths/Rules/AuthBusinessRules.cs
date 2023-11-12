using BusinessLayer.Features.Auths.Constants;
using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Models.Enums;
using WebAPI.Repositories.Abstract;
using WebAPI.Security.Constants;

namespace BusinessLayer.Features.Auths.Rules;

public class AuthBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;

    public AuthBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task OtpAuthenticatorShouldBeExists(OtpAuthenticator? otpAuthenticator)
    {
        // Checks if the OTP authenticator exists and throws an exception if it doesn't.
        // The OTP authenticator is considered to exist if it is not null.
        if (otpAuthenticator is null)
            throw new BusinessException(AuthMessages.OtpAuthenticatorDontExists);
        return Task.CompletedTask;
    }

    public Task UserShouldBeExists(User? user)
    {
        // Checks if the user exists and throws an exception if they don't.
        // The user is considered to exist if it is not null.
        if (user == null)
            throw new BusinessException(AuthMessages.UserDontExists);
        return Task.CompletedTask;
    }

    public Task HasTheEmailBeenSentSuccessfully(bool isSuccess)
    {
        if (isSuccess is false)
            throw new BusinessException(AppMessages.SendEmailError);
        return Task.CompletedTask;
    }

    public async Task ActivationCodeMustMatchTheOneInTheDatabase(int userId, string passwordResetKey)
    {
        User? result = await _userRepository.GetAsync(p => p.Id == userId && p.PasswordResetKey == passwordResetKey);
        if (result is null) throw new BusinessException(AuthMessages.PasswordResetActivationCodeIsNotCorrect);
    }

    public Task PasswordsEnteredMustBeTheSame(string newPassword, string repeatPassword)
    {
        // Checks if the entered passwords are the same and throws an exception if they are not.
        if (newPassword != repeatPassword)
            throw new BusinessException(AuthMessages.PasswordDoNotMatch);
        return Task.CompletedTask;
    }

    public Task EmailAuthenticatorShouldBeExists(EmailAuthenticator? emailAuthenticator)
    {
        // Checks if the email authenticator exists and throws an exception if it doesn't.
        // The email authenticator is considered to exist if it is not null.
        if (emailAuthenticator is null)
            throw new BusinessException(AuthMessages.EmailAuthenticatorDontExists);
        return Task.CompletedTask;
    }

    public Task TheUserMustHaveConfirmedTheirEmail(bool userStatus)
    {
        if (userStatus is false)
        {
            throw new BusinessException(AuthMessages.EmailNotConfirmed);
        }
        return Task.CompletedTask;
    }

    public Task EmailAuthenticatorActivationKeyShouldBeExists(EmailAuthenticator emailAuthenticator)
    {
        // Checks if the email authenticator has an activation key and throws an exception if it doesn't.
        // The activation key is considered to exist if it is not null.
        if (emailAuthenticator.ActivationKey is null)
            throw new BusinessException(AuthMessages.EmailActivationKeyDontExists);
        return Task.CompletedTask;
    }

    public Task AccountIsAlreadyActivated(bool isVerified)
    {
        // Checks if the email authenticator has an activation key and throws an exception if it doesn't.
        // The activation key is considered to exist if it is not null.
        if (isVerified is true)
            throw new BusinessException(AuthMessages.AccountIsAlreadyActivated);
        return Task.CompletedTask;
    }

    public async Task UserEmailShouldBeNotExists(string email)
    {
        // Checks if the provided email is available (not already used by another user) and throws an exception if it is not.
        User? user = await _userRepository.GetAsync(u => u.Email == email);
        if (user != null)
            throw new BusinessException(AuthMessages.UserEmailAlreadyExists);
    }

    public Task OtpAuthenticatorThatVerifiedShouldNotBeExists(OtpAuthenticator? otpAuthenticator)
    {
        // Checks if a verified OTP authenticator exists and throws an exception if it does.
        // The OTP authenticator is considered to be verified if it is not null and its IsVerified property is true.
        if (otpAuthenticator is not null && otpAuthenticator.IsVerified)
            throw new BusinessException(AuthMessages.AlreadyVerifiedOtpAuthenticatorIsExists);
        return Task.CompletedTask;
    }

    public async Task UserEmailMustBeAvailable(string email)
    {
        // Checks if the provided email is associated with an existing user and throws an exception if it is not.
        User? user = await _userRepository.GetAsync(u => u.Email == email);
        if (user == null)
            throw new BusinessException(AuthMessages.UserDontExists);
    }

    public async Task UserPasswordShouldBeMatch(int id, string password)
    {
        // Checks if the provided password matches the password of the user with the specified ID and throws an exception if it doesn't.
        User? user = await _userRepository.GetAsync(u => u.Id == id);
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(AuthMessages.PasswordDontMatch);
    }

    public Task RefreshTokenShouldBeActive(RefreshToken refreshToken)
    {
        // Checks if the refresh token is active (not revoked and not expired) and throws an exception ifit isn't.
        if (refreshToken.Revoked != null && DateTime.UtcNow >= refreshToken.Expires)
            throw new BusinessException(AuthMessages.InvalidRefreshToken);
        return Task.CompletedTask;
    }

    public Task RefreshTokenShouldBeExists(RefreshToken? refreshToken)
    {
        // Checks if the refresh token exists and throws an exception if it doesn't.
        // The refresh token is considered to exist if it is not null.
        if (refreshToken == null)
            throw new BusinessException(AuthMessages.RefreshDontExists);
        return Task.CompletedTask;
    }

    public Task UserShouldNotBeHaveAuthenticator(User user)
    {
        // Checks if the user does not have an authenticator assigned and throws an exception if they do.
        if (user.AuthenticatorType != AuthenticatorType.None)
            throw new BusinessException(AuthMessages.UserHaveAlreadyAAuthenticator);
        return Task.CompletedTask;
    }
}