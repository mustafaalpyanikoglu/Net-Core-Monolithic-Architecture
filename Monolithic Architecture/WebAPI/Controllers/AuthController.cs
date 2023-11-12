using BusinessLayer.Features.Auths.Commands.ForgotPasswordEmailAuthenticator;
using BusinessLayer.Features.Auths.Commands.RefleshToken;
using BusinessLayer.Features.Auths.Commands.Register;
using BusinessLayer.Features.Auths.Commands.RevokeToken;
using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Commands.ChangePassword;
using BusinessLayer.Features.Auths.Commands.Login;
using BusinessLayer.Services.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using static BusinessLayer.Features.Auths.Dtos.LoggedDto;
using BusinessLayer.Features.Auths.Commands.EnableEmailAuthenticator;
using BusinessLayer.Features.Auths.Commands.VerifyEmailAuthenticator;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.Models.Constants;
using WebAPI.Security.Jwt;
using WebAPI.Models.Concrete;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService; 
        private readonly WebAPIConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration.GetSection("WebAPIConfiguration").Get<WebAPIConfiguration>();
        }

        [ProducesResponseType(typeof(LoggedResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(description: ResponseDescriptions.AUTH_LOGIN)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            LoginCommand loginCommand = new() { UserForLoginDto = userForLoginDto, IPAddress = getIpAddress() };
            LoggedDto result = await Mediator.Send(loginCommand);

            if (result.RefreshToken is not null) setRefreshTokenToCookie(result.RefreshToken);

            return Ok(result.CreateResponseDto());
        }

        [ProducesResponseType(typeof(AccessToken), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.AUTH_REGISTER)]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            RegisterCommand registerCommand = new() { UserForRegisterDto = userForRegisterDto, IPAddress = getIpAddress() };
            RegisteredDto result = await Mediator.Send(registerCommand);
            setRefreshTokenToCookie(result.RefreshToken);
            return Created("", result.AccessToken);
        }

        [ProducesResponseType(typeof(RefreshedTokensDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(description: ResponseDescriptions.AUTH_REFRESHTOKEN)]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            RefreshTokenCommand refreshTokenCommand = new()
            { RefleshToken = getRefreshTokenFromCookies(), IPAddress = getIpAddress() };
            RefreshedTokensDto result = await Mediator.Send(refreshTokenCommand);
            setRefreshTokenToCookie(result.RefreshToken);
            return Created("", result.AccessToken);
        }

        [ProducesResponseType(typeof(RefreshedTokensDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
        string? refreshToken)
        {
            RevokeTokenCommand revokeTokenCommand = new()
            {
                Token = refreshToken ?? getRefreshTokenFromCookies(),
                IPAddress = getIpAddress()
            };
            RevokedTokenDto result = await Mediator.Send(revokeTokenCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(EnableEmailAuthenticatorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [HttpPost("EnableEmailAuthenticator")]
        public async Task<IActionResult> EnableEmailAuthenticator([FromBody]EnableEmailAuthenticatorRequestDto enableEmailAuthenticatorDto)
        {
            EnableEmailAuthenticatorCommand enableEmailAuthenticatorCommand =
                new() { Email = enableEmailAuthenticatorDto.Email, VerifyEmailUrlPrefix = $"{_configuration.ApiDomain}/Auth/VerifyEmailAuthenticator" };
            EnableEmailAuthenticatorResponseDto result =  await Mediator.Send(enableEmailAuthenticatorCommand);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ForgotPasswordEmailAuthenticatorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [HttpPost("ForgotPasswordEmailAuthenticator")]
        public async Task<IActionResult> ForgotPasswordEmailAuthenticator([FromBody] ForgotPasswordEmailAuthenticatorRequestDto forgotPasswordEmailAuthenticatorRequestDto)
        {
            ForgotPasswordEmailAuthenticatorCommand forgotPasswordEmailAuthenticatorCommand =
                new() { Email = forgotPasswordEmailAuthenticatorRequestDto.Email, VerifyEmailUrlPrefix = $"{_configuration.ApiDomain}/Auth/VerifyEmailAuthenticator" };
            ForgotPasswordEmailAuthenticatorResponseDto result = await Mediator.Send(forgotPasswordEmailAuthenticatorCommand);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [HttpPost("VerifyEmailAuthenticator")]
        public async Task<IActionResult> VerifyEmailAuthenticator([FromBody] VerifyEmailAuthenticatorCommand verifyEmailAuthenticatorCommand)
        {
            await Mediator.Send(verifyEmailAuthenticatorCommand);
            return Ok();
        }
        
        [ProducesResponseType(typeof(UserForChangePasswordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.AUTH_CHANGEPASSWORD)]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePasswordCommand)
        {
            UserForChangePasswordDto result = await Mediator.Send(changePasswordCommand);
            return Ok(result);
        }
        private string? getRefreshTokenFromCookies()
        {
            return Request.Cookies["refreshToken"];
        }

        private void setRefreshTokenToCookie(RefreshToken refreshToken)
        {
            CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        //[HttpGet("EnableOtpAuthenticator")]
        //public async Task<IActionResult> EnableOtpAuthenticator()
        //{
        //    EnableOtpAuthenticatorCommand enableOtpAuthenticatorCommand = new() { UserId = getUserIdFromRequest() };
        //    EnabledOtpAuthenticatorResponse result = await Mediator.Send(enableOtpAuthenticatorCommand);

        //    return Ok(result);
        //}



        //[HttpPost("VerifyOtpAuthenticator")]
        //public async Task<IActionResult> VerifyOtpAuthenticator([FromBody] string authenticatorCode)
        //{
        //    VerifyOtpAuthenticatorCommand verifyEmailAuthenticatorCommand =
        //        new() { UserId = getUserIdFromRequest(), ActivationCode = authenticatorCode };

        //    await Mediator.Send(verifyEmailAuthenticatorCommand);
        //    return Ok();
        //}
    }
}
