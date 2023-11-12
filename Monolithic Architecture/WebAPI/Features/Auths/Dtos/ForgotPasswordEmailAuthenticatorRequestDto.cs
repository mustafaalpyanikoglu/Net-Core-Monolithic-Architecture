
using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class ForgotPasswordEmailAuthenticatorRequestDto : IDto
{
    public string Email { get; set; }
}
