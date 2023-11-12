using WebAPI.Application.Dtos;
using WebAPI.Models.Concrete;
using WebAPI.Security.Jwt;

namespace BusinessLayer.Features.Auths.Dtos;

public class RefreshedTokensDto : IDto
{
    public AccessToken AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }
}