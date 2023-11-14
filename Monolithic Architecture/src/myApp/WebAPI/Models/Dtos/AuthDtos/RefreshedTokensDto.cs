using WebAPI.Models.Concrete;
using WebAPI.Security.Jwt;

namespace WebAPI.Models.Dtos.AuthDtos;

public class RefreshedTokensDto : IDto
{
    public AccessToken AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }
}
