using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class EnableEmailAuthenticatorRequestDto : IDto
{
    public string Email { get; set; }
}
