namespace BusinessLayer.Features.Auths.Constants;

public static class AuthMessages
{
    public const string EmailAuthenticatorDontExists = "Email authenticator don't exists.";
    public const string OtpAuthenticatorDontExists = "Otp authenticator don't exists.";
    public const string AlreadyVerifiedOtpAuthenticatorIsExists = "Already verified otp authenticator is exists.";
    public const string EmailActivationKeyDontExists = "Email Activation Key don't exists.";
    public const string UserDontExists = "User don't exists.";
    public const string UserHaveAlreadyAAuthenticator = "User have already a authenticator.";
    public const string RefreshDontExists = "Refresh don't exists.";
    public const string InvalidRefreshToken = "Invalid refresh token.";
    public const string UserEmailAlreadyExists = "User email already exists";
    public const string PasswordDontMatch = "Password don't match.";
    public const string PasswordChangedSuccessfully = "Şifre değiştirildi";
    public const string PasswordDoNotMatch = "passwords do not match";
    public const string EmailNotConfirmed = "email not confirmed";
    public const string AccountIsAlreadyActivated = "account is already activated";
    public const string PasswordResetActivationCodeIsNotCorrect = "password reset activation code is not correct";

}
