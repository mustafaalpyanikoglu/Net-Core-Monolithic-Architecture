using BusinessLayer.Features.Auths.Dtos;
using WebAPI.Application.Results.Abstract;
using WebAPI.Models.Concrete;
using WebAPI.Security.Jwt;

namespace BusinessLayer.Services.AuthService;

public interface IAuthService
{
    public Task<OtpAuthenticator> CreateOtpAuthenticator(User user);
    public Task<EmailAuthenticator> CreateEmailAuthenticator(User user);
    public Task<string> ConvertSecretKeyToString(byte[] secretKey);
    public Task<User> Register(UserForRegisterDto userForRegisterDto, string password);
    public Task<User> Login(UserForLoginDto userForLoginDto);
    public Task<RefreshToken?> GetRefreshTokenByToken(string token);
    public Task<User> ChangePassword(UserForChangePasswordDto userForChangePasswordDto);
    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
    public Task DeleteOldRefreshTokens(int userId);
    public Task<AccessToken> CreateAccessToken(User user);
    public Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);
    public Task SendAuthenticatorCode(User user);
    public Task VerifyAuthenticatorCode(User user, string authenticatorCode);
    public Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null,
                              string? replacedByToken = null);
    public Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress);
    public Task<IDataResult<string>> SendMail(string email);
    public Task<bool> SendVerificationMail(string email, string firstName, string activationKey);
}
