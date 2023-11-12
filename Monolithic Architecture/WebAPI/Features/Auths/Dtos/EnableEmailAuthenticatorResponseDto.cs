using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Auths.Dtos;

public class EnableEmailAuthenticatorResponseDto : IDto
{
    public int UserId { get; set; }
    public string ActivationKey { get; set; }
}
