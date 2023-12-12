using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contexts;
using static WebAPI.Models.Dtos.AuthDtos.LoggedDto;
using WebAPI.Models.Constants;
using WebAPI.Models.Dtos.AuthDtos;
using WebAPI.Models.Concrete;
using Microsoft.EntityFrameworkCore;
using static WebAPI.Models.Constants.ResponseDescriptions;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Security.Constants;
using WebAPI.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Security.Encyrption;
using System.Security.Claims;
using WebAPI.Security.SecurityExtensions;
using System.Security.Cryptography;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.Models.Enums;
using static WebAPI.Models.Constants.PathConstant;
using FluentValidation;


namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly WebAPI.Security.Jwt.TokenOptions _tokenOptions;
    private DateTime _accessTokenExpiration;
    private readonly IValidator<UserForRegisterDto> _userForRegisterDtoValidator;
    private readonly IValidator<UserForLoginDto> _userForLoginDtoValidator;

    public AuthController(
        IMapper mapper, 
        IConfiguration configuration, 
        IValidator<UserForRegisterDto> userForRegisterDtoValidator,
        IValidator<UserForLoginDto> userForLoginDtoValidator)
    {
        _mapper = mapper;
        _tokenOptions = configuration.GetSection("TokenOptions").Get<WebAPI.Security.Jwt.TokenOptions>();
        _userForRegisterDtoValidator = userForRegisterDtoValidator;
        _userForLoginDtoValidator = userForLoginDtoValidator;
    }


    [ProducesResponseType(typeof(AccessToken), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(description: ResponseDescriptions.AUTH_REGISTER)]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
        try
        {
            ValidationContext<UserForRegisterDto> validatorContext = new ValidationContext<UserForRegisterDto>(userForRegisterDto);
            IEnumerable<ValidationExceptionModel> errors = _userForRegisterDtoValidator.Validate(validatorContext)
                .Errors
                .Where(failure => failure != null)
                .GroupBy(
                    keySelector: p => p.PropertyName,
                    resultSelector: (propertyName, errors) =>
                        new ValidationExceptionModel { Property = propertyName, Errors = errors.Select(e => e.ErrorMessage) }
                )
                .ToList();
            if (errors.Any()) throw new WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException(errors);

            User? user;
            RegisteredDto registeredDto = new();
            using (var context = new BaseDbContext())
            {
                user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userForRegisterDto.Email);

                if (user is not null) throw new BusinessException(USER_EMAL_ALREADY_EXISTS);

                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);

                User newUser = new()
                {
                    Email = userForRegisterDto.Email,
                    FirstName = userForRegisterDto.FirstName,
                    LastName = userForRegisterDto.LastName,
                    PhoneNumber = userForRegisterDto.PhoneNumber,
                    Address = userForRegisterDto.Address,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    UserStatus = false,
                    ImageUrl = DEFAULT_IMAGE_URL,
                    RegistrationDate = DateTime.UtcNow,
                    AuthenticatorType = AuthenticatorType.Email,
                    PasswordResetKey = null,
                };

                context.Entry(newUser).State = EntityState.Added;
                await context.SaveChangesAsync();

                User? createdUser = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userForRegisterDto.Email);
                if (createdUser is null) throw new BusinessException(FAILED_TO_SAVE_USER);


                EmailAuthenticator emailAuthenticator = new()
                {
                    UserId = createdUser.Id,
                    ActivationKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    IsVerified = false
                };
                context.Entry(emailAuthenticator).State = EntityState.Added;

                IList<OperationClaim> operationClaims;
                operationClaims = await context.UserOperationClaims
                    .AsNoTracking()
                    .Where(p => p.UserId == createdUser.Id)
                    .Select(p => new OperationClaim
                    {
                        Id = p.OperationClaimId,
                        Name = p.OperationClaim.Name
                    }).ToListAsync();
                registeredDto.AccessToken = CreateToken(createdUser, operationClaims);
                registeredDto.AccessToken.Email = createdUser.Email;
                registeredDto.AccessToken.FirstName = createdUser.FirstName;
                registeredDto.AccessToken.LastName = createdUser.LastName;
                registeredDto.AccessToken.UserID = createdUser.Id;
                if (operationClaims.FirstOrDefault() is not null)
                    registeredDto.AccessToken.OperationClaimName = operationClaims.FirstOrDefault().Name;


                registeredDto.RefreshToken = CreateRefreshToken(createdUser, "");
                context.Entry(registeredDto.RefreshToken).State = EntityState.Added;

                IList<RefreshToken> refreshTokens = await context.RefreshTokens
                 .Where(r => r.UserId == createdUser.Id &&
                             r.Revoked == null && r.Expires >= DateTime.UtcNow &&
                             r.Created.AddDays(_tokenOptions.RefreshTokenTTL) <= DateTime.UtcNow)
                 .ToListAsync();
                foreach (RefreshToken refreshToken in refreshTokens) context.Entry(refreshToken).State = EntityState.Deleted;

                await context.SaveChangesAsync();
            }
            if (registeredDto.RefreshToken is not null) setRefreshTokenToCookie(registeredDto.RefreshToken);

            return Ok(registeredDto.AccessToken);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorModel()
            {
                Type = "https://example.com/probs/business",
                Title = "Rule Validation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = ""
            });

        }
        catch (WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException ex)
        {
            //return BadRequest(new WebAPI.CrossCuttingConcerns.Exceptions.HttpProblemDetails.ValidationProblemDetails(ex.Errors));
            return BadRequest(new ErrorModel()
            {
                Failures = ex.Errors.Select(validationException =>
                new Failure
                {
                    Property = validationException.Property ?? string.Empty,
                    Errors = validationException.Errors?.ToList() ?? new List<string>()
                }).ToList(),
                Type = "ValidationException",
                Title = "Validation Errors",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = ""
            });
        }
        catch (Exception ex)
        {
            // Diğer hata durumları...
            return StatusCode(500, new { message = SERVER_ERROR });
        }
    }


    [ProducesResponseType(typeof(LoggedResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(description: ResponseDescriptions.AUTH_LOGIN)]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
    {
        try
        {
            ValidationContext<UserForLoginDto> validatorContext = new ValidationContext<UserForLoginDto>(userForLoginDto);
            IEnumerable<ValidationExceptionModel> errors = _userForLoginDtoValidator.Validate(validatorContext)
                .Errors
                .Where(failure => failure != null)
                .GroupBy(
                    keySelector: p => p.PropertyName,
                    resultSelector: (propertyName, errors) =>
                        new ValidationExceptionModel { Property = propertyName, Errors = errors.Select(e => e.ErrorMessage) }
                )
                .ToList();
            if (errors.Any()) throw new WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException(errors);


            User? user;
            LoggedDto loggedDto = new();
            using (var context = new BaseDbContext())
            {
                user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userForLoginDto.Email);

                if (user is null) throw new BusinessException(EMAIL_NOT_FOUND);
                if (!user.UserStatus) throw new BusinessException(USER_DEACTIVE);
                if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt))
                    throw new BusinessException(PASSWORD_DONT_MATCH);

                IList<OperationClaim> operationClaims;
                operationClaims = await context.UserOperationClaims
                    .AsNoTracking()
                    .Where(p => p.UserId == user.Id)
                    .Select(p => new OperationClaim
                    {
                        Id = p.OperationClaimId,
                        Name = p.OperationClaim.Name
                    }).ToListAsync();
                loggedDto.AccessToken = CreateToken(user, operationClaims);
                loggedDto.AccessToken.Email = user.Email;
                loggedDto.AccessToken.FirstName = user.FirstName;
                loggedDto.AccessToken.LastName = user.LastName;
                loggedDto.AccessToken.UserID = user.Id;
                if (operationClaims.FirstOrDefault() is not null)
                    loggedDto.AccessToken.OperationClaimName = operationClaims.FirstOrDefault().Name;

                loggedDto.RefreshToken = CreateRefreshToken(user, "");

                context.Entry(loggedDto.RefreshToken).State = EntityState.Added;

                IList<RefreshToken> refreshTokens = await context.RefreshTokens
                 .Where(r => r.UserId == user.Id &&
                             r.Revoked == null && r.Expires >= DateTime.UtcNow &&
                             r.Created.AddDays(_tokenOptions.RefreshTokenTTL) <= DateTime.UtcNow)
                 .ToListAsync();
                foreach (RefreshToken refreshToken in refreshTokens) context.Entry(refreshToken).State = EntityState.Deleted;

                await context.SaveChangesAsync();
            }

            if (loggedDto.RefreshToken is not null) setRefreshTokenToCookie(loggedDto.RefreshToken);

            return Ok(loggedDto.CreateResponseDto());
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorModel()
            {
                Type = "https://example.com/probs/business",
                Title = "Rule Validation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = ""
            });

        }
        catch (WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException ex)
        {
            //return BadRequest(new WebAPI.CrossCuttingConcerns.Exceptions.HttpProblemDetails.ValidationProblemDetails(ex.Errors));
            return BadRequest(new ErrorModel()
            {
                Failures = ex.Errors.Select(validationException =>
                new Failure
                {
                    Property = validationException.Property ?? string.Empty,
                    Errors = validationException.Errors?.ToList() ?? new List<string>()
                }).ToList(),
                Type = "ValidationException",
                Title = "Validation Errors",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = ""
            });
        }
        catch (Exception ex)
        {
            // Diğer hata durumları...
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    private AccessToken CreateToken(User user, IList<OperationClaim> operationClaims)
    {
        _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken { Token = token, Expiration = _accessTokenExpiration };
    }

    private RefreshToken CreateRefreshToken(User user, string ipAddress)
    {
        RefreshToken refreshToken =
            new()
            {
                UserId = user.Id,
                Token = RandomRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress
            };

        return refreshToken;
    }

    private JwtSecurityToken CreateJwtSecurityToken(
        WebAPI.Security.Jwt.TokenOptions tokenOptions,
        User user,
        SigningCredentials signingCredentials,
        IList<OperationClaim> operationClaims
    )
    {
        JwtSecurityToken jwt =
            new(
                tokenOptions.Issuer,
                tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
            );
        return jwt;
    }


    private IEnumerable<Claim> SetClaims(User user, IList<OperationClaim> operationClaims)
    {
        List<Claim> claims = new() { };
        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddEmail(user.Email);
        claims.AddName($"{user.FirstName} {user.LastName}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
        return claims;
    }

    private string RandomRefreshToken()
    {
        byte[] numberByte = new byte[32];
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
        random.GetBytes(numberByte);
        return Convert.ToBase64String(numberByte);
    }
    private void setRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

}
