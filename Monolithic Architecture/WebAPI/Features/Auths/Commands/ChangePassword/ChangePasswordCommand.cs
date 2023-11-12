using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Auths.Dtos;
using BusinessLayer.Features.Auths.Rules;
using MediatR;
using WebAPI.Models.Concrete;
using WebAPI.Security.Constants;

namespace BusinessLayer.Features.Auths.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<UserForChangePasswordDto>
{
    public string Email { get; set; }
    public string PasswordResetKey { get; set; }
    public string NewPassword { get; set; }
    public string RepeatPassword { get; set; }


    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, UserForChangePasswordDto>
    {
        private readonly IMapper _mapper;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IMapper mapper, AuthBusinessRules authBusinessRules, IUserRepository userRepository)
        {
            _mapper = mapper;
            _authBusinessRules = authBusinessRules;
            _userRepository = userRepository;
        }

        public async Task<UserForChangePasswordDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetAsync(u => u.Email == request.Email);
            await _authBusinessRules.UserShouldBeExists(user);

            await _authBusinessRules.ActivationCodeMustMatchTheOneInTheDatabase(user.Id, request.PasswordResetKey);
            await _authBusinessRules.PasswordsEnteredMustBeTheSame(request.NewPassword, request.RepeatPassword);

            byte[] passwordHash, passwordSalt;

            HashingHelper.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.PasswordResetKey = null;
            User updatedUser = await _userRepository.UpdateAsync(user);

            UserForChangePasswordDto updatedUserDto = _mapper.Map<UserForChangePasswordDto>(updatedUser);

            updatedUserDto.Password = request.NewPassword;

            return updatedUserDto;
        }
    }
}
