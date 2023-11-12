using Microsoft.EntityFrameworkCore;
using WebAPI.Security.Jwt;
using WebAPI.Security.EmailAuthenticator;
using MimeKit;
using Core.Mailing;
using WebAPI.Security.OtpAuthenticator;
using static BusinessLayer.Features.Auths.Constants.AuthMessages;
using static BusinessLayer.Features.Users.Constants.UserMessages;
using System.Net.Mail;
using System.Net;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Rules;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Application.Results.Abstract;
using WebAPI.Application.Results.Concrete;
using WebAPI.Security.Constants;
using WebAPI.Models.Enums;

namespace BusinessLayer.Services.AuthService;

public class AuthManager : IAuthService
{
    #region Definition of services
    private readonly ITokenHelper _tokenHelper;
    private readonly AuthBusinessRules _authBusinessRules;
    private readonly IUserRepository _userDal;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;
    private readonly IMailService _mailService;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
    private readonly IOtpAuthenticatorHelper _otpAuthenticatorHelper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenOptions _tokenOptions;
    private readonly SmtpSettings _smtpSettings;
    #endregion

    public AuthManager(ITokenHelper tokenHelper, AuthBusinessRules authBusinessRules,
        IUserRepository userDal, IUserOperationClaimRepository userOperationClaimRepository,
        IEmailAuthenticatorRepository emailAuthenticatorRepository,
        IEmailAuthenticatorHelper emailAuthenticatorHelper, IMailService mailService,
        IOtpAuthenticatorRepository otpAuthenticatorRepository, IOtpAuthenticatorHelper otpAuthenticatorHelper,
        IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
    {
        _tokenHelper = tokenHelper;
        _authBusinessRules = authBusinessRules;
        _userDal = userDal;
        _userOperationClaimRepository = userOperationClaimRepository;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
        _emailAuthenticatorHelper = emailAuthenticatorHelper;
        _mailService = mailService;
        _otpAuthenticatorRepository = otpAuthenticatorRepository;
        _otpAuthenticatorHelper = otpAuthenticatorHelper;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
        _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
    }

    public async Task<IDataResult<string>> SendMail(string email)
    {
        User? user = await _userDal.GetAsync(p => p.Email == email);
        if (user is not null)
        {
            EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(p => p.UserId == user.Id);
            if (emailAuthenticator is null)
            {
                emailAuthenticator = await CreateEmailAuthenticator(user);
            }

            await _authBusinessRules.AccountIsAlreadyActivated(emailAuthenticator.IsVerified);

            bool isMailSent = await SendVerificationMail(email, user.FirstName, emailAuthenticator.ActivationKey);
            if (isMailSent)
            {
                return new SuccessDataResult<string>(emailAuthenticator.ActivationKey, AppMessages.SendEmail);
            }
            else
            {
                return new ErrorDataResult<string>(AppMessages.SendEmailError);
            }
        }
        else
        {
            return new ErrorDataResult<string>(UserNotFound);
        }
    }

    public async Task<bool> SendVerificationMail(string email, string firstName, string activationKey)
    {
        try
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(email);
                mailMessage.To.Add(email);
                mailMessage.Subject = "MONOVI - Staj Takip Programı";
                mailMessage.Body = $"<h2>Merhaba {firstName}</h2>" + "<p>Aktivasyon kodunuz çift tırnak içindeki ifadedir: " + $"\"<b>{activationKey}</b>\"" + "</p>";
                mailMessage.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            // Hata işleme veya günlüğe yazma işlemleri burada yapılabilir
            return false;
        }
    }

    public async Task<string> ConvertSecretKeyToString(byte[] secretKey)
    {
        // Converts a secret key from byte array to string format
        string result = await _otpAuthenticatorHelper.ConvertSecretKeyToString(secretKey);
        return result;
    }
    public async Task<OtpAuthenticator> CreateOtpAuthenticator(User user)
    {
        // Creates an OTP (One-Time Password) authenticator for the given user
        // Generates a secret key and assigns it to the authenticator
        OtpAuthenticator otpAuthenticator =
            new()
            {
                UserId = user.Id,
                SecretKey = await _otpAuthenticatorHelper.GenerateSecretKey(),
                IsVerified = false
            };
        return otpAuthenticator;
    }
    public async Task<EmailAuthenticator> CreateEmailAuthenticator(User user)
    {
        // Creates an email authenticator for the given user
        // Generates an activation key and assigns it to the authenticator
        EmailAuthenticator emailAuthenticator =
            new()
            {
                UserId = user.Id,
                ActivationKey = await _emailAuthenticatorHelper.CreateEmailActivationKey(),
                IsVerified = false
            };
        return emailAuthenticator;
    }
    public async Task<User> ChangePassword(UserForChangePasswordDto userForChangePasswordDto)
    {
        // Changes the password for the user specified in the DTO (Data Transfer Object)
        // Retrieves the user based on the email address provided
        // Updates the user's password hash and salt with the new password
        byte[] passwordHash, passwordSalt;

        User? userResult = await _userDal.GetAsync(u => u.Email == userForChangePasswordDto.Email);
        await _authBusinessRules.UserShouldBeExists(userResult);

        HashingHelper.CreatePasswordHash(userForChangePasswordDto.Password, out passwordHash, out passwordSalt);
        userResult.PasswordHash = passwordHash;
        userResult.PasswordSalt = passwordSalt;
        await _userDal.UpdateAsync(userResult);

        return await Task.FromResult(userResult);
    }
    public async Task<AccessToken> CreateAccessToken(User user)
    {
        // Creates an access token for the specified user
        // Retrieves the operation claims associated with the user
        // Creates an access token using a token helper and returns it
        IList<OperationClaim> operationClaims = await _userOperationClaimRepository
                .Query()
                .AsNoTracking()
                .Where(p => p.UserId == user.Id)
                .Select(p => new OperationClaim
                {
                    Id = p.OperationClaimId,
                    Name = p.OperationClaim.Name
                })
                .ToListAsync();

        AccessToken accessToken = _tokenHelper.CreateToken(user, operationClaims);
        accessToken.Email = user.Email;
        accessToken.FirstName = user.FirstName;
        accessToken.LastName = user.LastName;
        accessToken.UserID = user.Id;
        if (operationClaims.FirstOrDefault() is not null)
            accessToken.OperationClaimName = operationClaims.FirstOrDefault().Name;
        return accessToken;
    }
    public async Task<User> Login(UserForLoginDto userForLoginDto)
    {
        // Authenticates a user during the login process
        // Retrieves the user based on the provided email address
        // Validates the user's existence and checks if the provided password matches the stored password
        // Returns the authenticated user or null if the authentication fails
        User? user = await _userDal.GetAsync(u => u.Email == userForLoginDto.Email);
        await _authBusinessRules.UserShouldBeExists(user);
        await _authBusinessRules.UserPasswordShouldBeMatch(user.Id, userForLoginDto.Password);

        if (user == null)
        {
            return null;
        }
        if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password ?? "", user.PasswordHash, user.PasswordSalt))
        {
            return await Task.FromResult(user);
        }
        return await Task.FromResult(user);
    }
    public async Task<User> Register(UserForRegisterDto userForRegisterDto, string password)
    {
        // Registers a new user with the provided details and password
        // Checks if theprovided email address is not already registered
        // Creates a password hash and salt for the user's password
        // Creates a new User object with the provided details and assigns the password hash and salt
        // Adds the user to the database and returns the registered user
        await _authBusinessRules.UserEmailShouldBeNotExists(userForRegisterDto.Email);

        byte[] passwordHash, passwordSalt;
        HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
        User user = new User
        {
            FirstName = userForRegisterDto.FirstName,
            LastName = userForRegisterDto.LastName,
            PhoneNumber = userForRegisterDto.PhoneNumber,
            Address = userForRegisterDto.Address,
            Email = userForRegisterDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            UserStatus = false,
            ImageUrl = null,
            RegistrationDate = Convert.ToDateTime(DateTime.Now.ToString("F"))
        };

        await _userDal.AddAsync(user);


        return await Task.FromResult(user);
    }
    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
    {
        // Retrieves a refresh token based on the provided token string
        RefreshToken? refreshToken = await _refreshTokenRepository.GetAsync(r => r.Token == token);
        return refreshToken;
    }
    public async Task SendAuthenticatorCode(User user)
    {
        // Sends the authenticator code to the user
        // If the user's authenticator type is Email, sends the code via email
        if (user.AuthenticatorType is AuthenticatorType.Email) await sendAuthenticatorCodeWithEmail(user);
    }
    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
    {
        // Creates a refresh token for the specified user and IP address
        RefreshToken refreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        return Task.FromResult(refreshToken);
    }
    public async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        // Rotates the refresh token for the specified user and IP address
        // Generates a new refresh token and revokes the old one
        RefreshToken newRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        await RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }
    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        // Adds a new refresh token to the database
        RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);
        return addedRefreshToken;
    }
    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress,
                                                string reason)
    {
        // Revokes all descendant refresh tokens of the provided refresh token
        // Recursively revokes child tokens until there are no more child tokens
        RefreshToken childToken = await _refreshTokenRepository.GetAsync(r => r.Token == refreshToken.ReplacedByToken);

        if (childToken != null && childToken.Revoked != null && childToken.Expires <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else await RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
    }
    public async Task DeleteOldRefreshTokens(int userId)
    {
        // Deletes old refresh tokens for the specified user
        // Retrieves a list of refresh tokens that are not revoked and have expired
        // Deletes each refresh token from the database
        IList<RefreshToken> refreshTokens = (await _refreshTokenRepository.GetListAsync(r =>
                                                 r.UserId == userId &&
                                                 r.Revoked == null && r.Expires >= DateTime.UtcNow &&
                                                 r.Created.AddDays(_tokenOptions.RefreshTokenTTL) <=
                                                 DateTime.UtcNow)
                                            ).Items;
        foreach (RefreshToken refreshToken in refreshTokens) await _refreshTokenRepository.DeleteAsync(refreshToken);
    }
    private async Task sendAuthenticatorCodeWithEmail(User user)
    {
        // Sends the authenticator code to the user via email
        // Retrieves the email authenticator associated with the user
        // Generates an authenticator code and updates the activation key
        // Sends an email containing the authenticator code to the user's email address
        EmailAuthenticator emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);

        if (!emailAuthenticator.IsVerified) throw new BusinessException("Email Authenticator must be is verified.");

        string authenticatorCode = await _emailAuthenticatorHelper.CreateEmailActivationCode();
        emailAuthenticator.ActivationKey = authenticatorCode;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);

        var toEmailList = new List<MailboxAddress>
        {
            new($"{user.FirstName} {user.LastName}",user.Email)
        };

        _mailService.SendMail(new Mail
        {
            ToList = toEmailList,
            Subject = "Authenticator Code - Monovi",
            TextBody = $"Enter your authenticator code: {authenticatorCode}"
        });
    }
    public async Task VerifyAuthenticatorCode(User user, string authenticatorCode)
    {
        // Verifies the authenticator code provided by the user
        // If the user's authenticator type is Email, verifies the code with email authentication
        // If the user's authenticator type is OTP, verifies the code with OTP authentication
        if (user.AuthenticatorType is AuthenticatorType.Email)
            await verifyAuthenticatorCodeWithEmail(user, authenticatorCode);
        else if (user.AuthenticatorType is AuthenticatorType.Otp)
            await verifyAuthenticatorCodeWithOtp(user, authenticatorCode);
    }
    private async Task verifyAuthenticatorCodeWithEmail(User user, string authenticatorCode)
    {
        // Verifies the authenticator code with email authentication
        // Retrieves the email authenticator associated with the user
        // Checks if the provided authenticator code matches the activation key
        // Updates the activation key to null if the code is valid
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);

        if (emailAuthenticator is null)
            throw new BusinessException("Authenticator not found.");

        if (emailAuthenticator.ActivationKey != authenticatorCode)
            throw new BusinessException("Authenticator code is invalid.");

        emailAuthenticator.ActivationKey = null;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }
    private async Task verifyAuthenticatorCodeWithOtp(User user, string authenticatorCode)
    {
        // Verifies the authenticator code with OTP authentication
        // Retrieves the OTP authenticator associated with the user
        // Verifies the code using the OTP authenticator's secret key
        OtpAuthenticator otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);

        bool result = await _otpAuthenticatorHelper.VerifyCode(otpAuthenticator.SecretKey, authenticatorCode);

        if (!result)
            throw new BusinessException("Authenticator code is invalid.");
    }
    public async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null,
                                     string? replacedByToken = null)
    {
        // Revokes a refresh token and updates its properties
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }
}
