using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class UserForLoginDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
