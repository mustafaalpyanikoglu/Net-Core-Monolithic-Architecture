using WebAPI.Application.Responses;

namespace BusinessLayer.Features.Auths.Commands.EnableOtpAuthenticator;

public class EnabledOtpAuthenticatorResponse : IResponse
{
    public string SecretKey { get; set; }
}
