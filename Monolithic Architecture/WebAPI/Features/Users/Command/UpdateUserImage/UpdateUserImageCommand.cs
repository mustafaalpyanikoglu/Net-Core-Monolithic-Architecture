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

namespace BusinessLayer.Features.Users.Command.UpdateUserImage;

public class UpdateUserImageCommand : IRequest<UpdatedUserResponseDto>/*,ISecuredRequest*/
{
    #region Request Requirements
    public int Id { get; set; }
    public IFormFile? File { get; set; }
    #endregion

    //public string[] Roles => new[] { Admin, UserUpdate };

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserImageCommand, UpdatedUserResponseDto>
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

        public async Task<UpdatedUserResponseDto> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userDal.GetAsync(u => u.Id == Convert.ToInt32(request.Id));
            await _userBusinessRules.UserShouldBeExist(user);

            if (request.File is not null) user.ImageUrl = await _userService.UpdateImage(request.File, user.ImageUrl);
            else user.ImageUrl = user.ImageUrl;

            User updatedUser = await _userDal.UpdateAsync(user);
            UpdatedUserResponseDto updatedUserDto = _mapper.Map<UpdatedUserResponseDto>(updatedUser);

            return updatedUserDto;
        }
    }
}
