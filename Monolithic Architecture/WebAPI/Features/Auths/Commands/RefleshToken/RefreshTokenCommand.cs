using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Rules;
using BusinessLayer.Services.AuthService;
using BusinessLayer.Services.UserService;
using WebAPI.Security.Jwt;
using WebAPI.Models.Concrete;
using MediatR;

namespace BusinessLayer.Features.Auths.Commands.RefleshToken;

public class RefreshTokenCommand : IRequest<RefreshedTokensDto>
{
    public string? RefleshToken { get; set; }
    public string? IPAddress { get; set; }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshedTokensDto>
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RefreshTokenCommandHandler(IAuthService authService, IUserService userService,
                                          AuthBusinessRules authBusinessRules)
        {
            _authService = authService;
            _userService = userService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<RefreshedTokensDto> Handle(RefreshTokenCommand request,
                                                     CancellationToken cancellationToken)
        {
            RefreshToken? refreshToken = await _authService.GetRefreshTokenByToken(request.RefleshToken);

            // Check if the refresh token exists
            await _authBusinessRules.RefreshTokenShouldBeExists(refreshToken);

            // If the refresh token is revoked, prevent reuse and log the attempt
            if (refreshToken.Revoked != null)
                await _authService.RevokeDescendantRefreshTokens(refreshToken, request.IPAddress, $"Attempted reuse of revoked ancestor token: {refreshToken.Token}");

            // Check if the refresh token is active
            await _authBusinessRules.RefreshTokenShouldBeActive(refreshToken);

            // Retrieve the user associated with the refresh token
            User user = await _userService.GetById(refreshToken.UserId);

            // Rotate the refresh token for enhanced security
            RefreshToken newRefreshToken = await _authService.RotateRefreshToken(user, refreshToken, request.IPAddress);

            // Add the new refresh token to the database
            RefreshToken addedRefreshToken = await _authService.AddRefreshToken(newRefreshToken);

            // Delete old refresh tokens for the user
            await _authService.DeleteOldRefreshTokens(refreshToken.UserId);

            // Create a new access token for the user
            AccessToken createdAccessToken = await _authService.CreateAccessToken(user);

            // Create a RefreshedTokensDto object with the new access token and refresh token
            RefreshedTokensDto refreshedTokensDto = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = addedRefreshToken
            };

            return refreshedTokensDto;
        }
    }
}