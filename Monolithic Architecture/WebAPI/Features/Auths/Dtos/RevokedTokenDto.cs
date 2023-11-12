using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class RevokedTokenDto : IDto
{
    public int Id { get; set; }
    public string Token { get; set; }
}