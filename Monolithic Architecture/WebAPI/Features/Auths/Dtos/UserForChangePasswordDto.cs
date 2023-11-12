using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class UserForChangePasswordDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
