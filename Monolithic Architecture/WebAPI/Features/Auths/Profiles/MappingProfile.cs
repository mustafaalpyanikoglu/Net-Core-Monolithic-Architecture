using AutoMapper;
using BusinessLayer.Features.Auths.Commands.ChangePassword;
using BusinessLayer.Features.Auths.Commands.EnableEmailAuthenticator;
using BusinessLayer.Features.Auths.Commands.ForgotPasswordEmailAuthenticator;
using BusinessLayer.Features.Auths.Dtos;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Auths.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, ChangePasswordCommand>().ReverseMap();
        CreateMap<User, UserForChangePasswordDto>().ReverseMap();
        CreateMap<RefreshToken, RevokedTokenDto>().ReverseMap();
        CreateMap<EnableEmailAuthenticatorCommand, EnableEmailAuthenticatorResponseDto>().ReverseMap();
        CreateMap<EmailAuthenticator, EnableEmailAuthenticatorResponseDto>().ReverseMap();
        CreateMap<ForgotPasswordEmailAuthenticatorCommand, ForgotPasswordEmailAuthenticatorResponseDto>().ReverseMap();
        CreateMap<EmailAuthenticator, ForgotPasswordEmailAuthenticatorResponseDto>().ReverseMap();
        CreateMap<User, ForgotPasswordEmailAuthenticatorResponseDto>().ReverseMap();
    }
}
