using FluentValidation;

namespace WebAPI.Models.Dtos.AuthDtos;

public class UserForLoginDtoValidator : AbstractValidator<UserForLoginDto>
{
    public UserForLoginDtoValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(3);
    }
}