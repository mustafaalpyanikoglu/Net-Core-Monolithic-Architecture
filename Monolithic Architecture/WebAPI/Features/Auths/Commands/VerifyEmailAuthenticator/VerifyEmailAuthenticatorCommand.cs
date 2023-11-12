using WebAPI.Repositories.Abstract;
using BusinessLayer.Features.Auths.Rules;
using WebAPI.Models.Concrete;
using MediatR;
using BusinessLayer.Services.UserService;

namespace BusinessLayer.Features.Auths.Commands.VerifyEmailAuthenticator;

public class VerifyEmailAuthenticatorCommand : IRequest
{
    public string ActivationKey { get; set; }

    public class VerifyEmailAuthenticatorCommandHandler : IRequestHandler<VerifyEmailAuthenticatorCommand>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
        private readonly IUserService _userService;

        public VerifyEmailAuthenticatorCommandHandler(
            AuthBusinessRules authBusinessRules,
            IEmailAuthenticatorRepository emailAuthenticatorRepository,
            IUserService userService)
        {
            _authBusinessRules = authBusinessRules;
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
            _userService = userService;
        }

        public async Task Handle(VerifyEmailAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(
                e => e.ActivationKey == request.ActivationKey
            );
            await _authBusinessRules.EmailAuthenticatorShouldBeExists(emailAuthenticator);
            await _authBusinessRules.EmailAuthenticatorActivationKeyShouldBeExists(emailAuthenticator);

            emailAuthenticator.ActivationKey = null;
            emailAuthenticator.IsVerified = true;
            await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);

            await _userService.ActivateTheUser(emailAuthenticator.UserId);

        }
    }
}
