using WebAPI.Repositories.Abstract;
using AutoMapper;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static WebAPI.Models.Constants.OperationClaims;
using static WebAPI.Models.Constants.PathConstant;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using BusinessLayer.Services.UserService;

namespace BusinessLayer.Features.Users.Command.ResetUserImage;

public class ResetUserImageCommand : IRequest<ResetUserImageRequestDto>/*,ISecuredRequest*/
{
    #region Request Requirements
    public int Id { get; set; }
    #endregion

    public string[] Roles => new[] { ADMIN, INTERN, USER };

    public class ResetUserImageCommandHandler : IRequestHandler<ResetUserImageCommand, ResetUserImageRequestDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserService _userService;

        public ResetUserImageCommandHandler(IUserRepository userDal, IMapper mapper, UserBusinessRules userBusinessRules, IUserService userService)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userService = userService;
        }

        public async Task<ResetUserImageRequestDto> Handle(ResetUserImageCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userDal.GetAsync(u => u.Id == request.Id);
            await _userBusinessRules.UserShouldBeExist(user);

            user.ImageUrl = DEFAULT_IMAGE_URL;

            User updatedUser = await _userDal.UpdateAsync(user);
            ResetUserImageRequestDto updatedUserDto = _mapper.Map<ResetUserImageRequestDto>(updatedUser);

            return updatedUserDto;
        }
    }
}
