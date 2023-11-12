using WebAPI.Repositories.Abstract;
using WebAPI.Security.Jwt;
using WebAPI.Models.Concrete;
using WebAPI.Models.Enums;
using MediatR;
using AccessToken = WebAPI.Security.Jwt.AccessToken;
using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Rules;
using BusinessLayer.Services.AuthService;
using BusinessLayer.Services.UserService;

namespace BusinessLayer.Features.Auths.Commands.Login;

public class LoginCommand : IRequest<LoggedDto>
{
    public UserForLoginDto UserForLoginDto { get; set; }
    public string IPAddress { get; set; }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedDto>
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;

        public LoginCommandHandler(IUserService userService, IAuthService authService, AuthBusinessRules authBusinessRules, IEmailAuthenticatorRepository emailAuthenticatorRepository)
        {
            _userService = userService;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
        }

        public async Task<LoggedDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetByEmail(request.UserForLoginDto.Email);
            await _authBusinessRules.UserShouldBeExists(user);
            await _authBusinessRules.TheUserMustHaveConfirmedTheirEmail(user.UserStatus);

            // Check if the user exists
            await _authBusinessRules.UserShouldBeExists(user);

            // Check if the entered password matches the user's password
            await _authBusinessRules.UserPasswordShouldBeMatch(user.Id, request.UserForLoginDto.Password);

            LoggedDto loggedDto = new();

            // Create an access token for the user
            AccessToken createdAccessToken = await _authService.CreateAccessToken(user);

            // Create a refresh token for the user and IP address
            RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(user, request.IPAddress);

            // Add the refresh token to the database
            RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);

            // Delete old refresh tokens for the user
            await _authService.DeleteOldRefreshTokens(user.Id);

            // Set the access token and refresh token in the LoggedDto
            loggedDto.AccessToken = createdAccessToken;
            loggedDto.RefreshToken = addedRefreshToken;

            return loggedDto;
        }
    }
}
