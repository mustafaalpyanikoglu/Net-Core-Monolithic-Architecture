using WebAPI.Repositories.Abstract;
using AutoMapper;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using static BusinessLayer.Features.Users.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using BusinessLayer.Services.UserService;

namespace BusinessLayer.Features.Users.Command.UpdateUser;

public class UpdateUserCommand : IRequest<UpdatedUserResponseDto>/*,ISecuredRequest*/
{
    #region Request Requirements
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public IFormFile? Image { get; set; }
    #endregion

    //public string[] Roles => new[] { Admin, UserUpdate };

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdatedUserResponseDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserRepository userDal, IMapper mapper, UserBusinessRules userBusinessRules, IUserService userService)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userService = userService;
        }

        public async Task<UpdatedUserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userDal.GetAsync(u => u.Id == request.Id);
            await _userBusinessRules.UserShouldBeExist(user);

            User mappedUser = _mapper.Map<User>(request);
            mappedUser.AuthenticatorType = user.AuthenticatorType;
            mappedUser.PasswordHash = user.PasswordHash;
            mappedUser.PasswordSalt = user.PasswordSalt;

            if (request.Image is not null)
            {
                mappedUser.ImageUrl = await _userService.UpdateImage(request.Image, mappedUser.ImageUrl);
            }
            else
            {
                mappedUser.ImageUrl = user.ImageUrl;
            }


            User updatedUser = await _userDal.UpdateAsync(mappedUser);
            UpdatedUserResponseDto updatedUserDto = _mapper.Map<UpdatedUserResponseDto>(updatedUser);

            // Returning the updated user DTO
            return updatedUserDto;
        }
    }
}
