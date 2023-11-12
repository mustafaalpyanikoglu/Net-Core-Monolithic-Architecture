using FluentValidation;

namespace BusinessLayer.Features.Auths.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Must(ContainUppercase)
            .WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Must(ContainLowercase)
            .WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Must(ContainDigit)
            .WithMessage("Şifre en az bir rakam içermelidir.")
            .Must(ContainSymbol)
            .WithMessage("Şifre en az bir sembol içermelidir.")
            .WithMessage("Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir sembol içermelidir.");
    }

    private bool ContainUppercase(string password)
    {
        return password.Any(c => char.IsUpper(c));
    }

    private bool ContainLowercase(string password)
    {
        return password.Any(c => char.IsLower(c));
    }

    private bool ContainDigit(string password)
    {
        return password.Any(c => char.IsDigit(c));
    }

    private bool ContainSymbol(string password)
    {
        return password.Any(c => !char.IsLetterOrDigit(c));
    }
}
