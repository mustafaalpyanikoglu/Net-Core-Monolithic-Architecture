using WebAPI.Repositories.Abstract;
using AutoMapper;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Users.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using BusinessLayer.Services.AuthService;
using WebAPI.Security.Constants;

namespace BusinessLayer.Features.Users.Command.UpdateUserFromAuth;

public class UpdateUserFromAuthCommand : IRequest<UpdatedUserFromAuthDto>
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string? NewPassword { get; set; }


    public class UpdateUserFromAuthCommandHandler : IRequestHandler<UpdateUserFromAuthCommand, UpdatedUserFromAuthDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IAuthService _authService;

        public UpdateUserFromAuthCommandHandler(IUserRepository userDal, IMapper mapper, UserBusinessRules userBusinessRules, IAuthService authService)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _authService = authService;
        }

        public async Task<UpdatedUserFromAuthDto> Handle(UpdateUserFromAuthCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userDal.GetAsync(u => u.Id == request.Id);

            // Checking if the user exists
            await _userBusinessRules.UserShouldBeExist(user);

            // Checking if the provided password matches the user's password
            await _userBusinessRules.UserPasswordShouldBeMatch(user, request.Password);

            // Updating the user's first name and last name
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            // Checking if a new password is provided and updating the password if necessary
            if (request.NewPassword is not null && !string.IsNullOrWhiteSpace(request.NewPassword))
            {
                // Creating a new password hash and salt
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // Updating the user in the data access layer (DAL)
            User updatedUser = await _userDal.UpdateAsync(user);

            // Mapping the updated user object to an UpdatedUserFromAuthDto object
            UpdatedUserFromAuthDto updatedUserFromAuthDto = _mapper.Map<UpdatedUserFromAuthDto>(updatedUser);

            // Creating a new access token for the user
            updatedUserFromAuthDto.AccessToken = await _authService.CreateAccessToken(user);

            // Returning the updated user DTO with the access token
            return updatedUserFromAuthDto;
        }
    }
}
