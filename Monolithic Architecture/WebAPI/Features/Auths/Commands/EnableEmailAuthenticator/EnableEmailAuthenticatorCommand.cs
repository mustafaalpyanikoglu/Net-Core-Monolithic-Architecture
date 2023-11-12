using WebAPI.Repositories.Abstract;
using AutoMapper;
using Core.Mailing;
using WebAPI.Models.Concrete;
using WebAPI.Models.Enums;
using MediatR;
using MimeKit;
using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Rules;
using BusinessLayer.Services.AuthService;
using BusinessLayer.Services.UserService;

namespace BusinessLayer.Features.Auths.Commands.EnableEmailAuthenticator;

public class EnableEmailAuthenticatorCommand : IRequest<EnableEmailAuthenticatorResponseDto>
{
    public string Email { get; set; }
    public string VerifyEmailUrlPrefix { get; set; }

    public class EnableEmailAuthenticatorCommandHandler : IRequestHandler<EnableEmailAuthenticatorCommand, EnableEmailAuthenticatorResponseDto>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IAuthService _authenticatorService;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EnableEmailAuthenticatorCommandHandler(AuthBusinessRules authBusinessRules, IAuthService authenticatorService, IEmailAuthenticatorRepository emailAuthenticatorRepository, IUserService userService, IMapper mapper)
        {
            _authBusinessRules = authBusinessRules;
            _authenticatorService = authenticatorService;
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<EnableEmailAuthenticatorResponseDto> Handle(EnableEmailAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetByEmail(request.Email);
            await _authBusinessRules.UserShouldBeExists(user);

            EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(p => p.UserId == user.Id);
            EmailAuthenticator createEmailAuthenticator = await _authenticatorService.CreateEmailAuthenticator(user);
            if (emailAuthenticator is null)
            {
                emailAuthenticator = await _emailAuthenticatorRepository.AddAsync(createEmailAuthenticator);
            }
            else
            {
                await _authBusinessRules.AccountIsAlreadyActivated(emailAuthenticator.IsVerified);

                emailAuthenticator.ActivationKey = createEmailAuthenticator.ActivationKey;
                emailAuthenticator.IsVerified = createEmailAuthenticator.IsVerified;
                await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
            }

            bool isTheMailSuccess = await _authenticatorService.SendVerificationMail(user.Email, user.FirstName, emailAuthenticator.ActivationKey);
            await _authBusinessRules.HasTheEmailBeenSentSuccessfully(isTheMailSuccess);

            await _authenticatorService.SendVerificationMail(user.Email, user.FirstName, emailAuthenticator.ActivationKey);

            EnableEmailAuthenticatorResponseDto enableEmailAuthenticatorResponseDto = _mapper.Map<EnableEmailAuthenticatorResponseDto>(createEmailAuthenticator);

            return enableEmailAuthenticatorResponseDto;
        }
    }
}
