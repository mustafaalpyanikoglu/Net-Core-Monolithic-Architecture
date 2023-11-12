using WebAPI.Application.Dtos;
using WebAPI.Models.Concrete;
using WebAPI.Security.Jwt;

namespace BusinessLayer.Features.Auths.Dtos;

public class LoggedDto : IDto
{
    public AccessToken? AccessToken { get; set; }
    public RefreshToken? RefreshToken { get; set; }

    public LoggedResponseDto CreateResponseDto()
    {
        return new() { AccessToken = AccessToken };
    }

    public class LoggedResponseDto
    {
        public AccessToken? AccessToken { get; set; }
    }
}