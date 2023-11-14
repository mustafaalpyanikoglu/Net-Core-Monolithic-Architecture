using WebAPI.Models.Concrete;
using WebAPI.Security.Jwt;

namespace WebAPI.Models.Dtos.AuthDtos;

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
