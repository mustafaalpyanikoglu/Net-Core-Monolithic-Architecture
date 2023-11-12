using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Users.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Users.Command.DeleteUser;

public class DeleteUserCommand : IRequest<DeletedUserDto>/*,ISecuredRequest*/
{
    public int Id { get; set; }

    //public string[] Roles => new[] { Admin, UserDelete };

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeletedUserDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public DeleteUserCommandHandler(IUserRepository userDal, IMapper mapper, UserBusinessRules userBusinessRules)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<DeletedUserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdMustBeAvailable(request.Id);

            User user = await _userDal.GetAsync(p => p.Id == request.Id);
            user.UserStatus = false;

            // Updating the user's status to mark it as deleted in the data access layer (DAL)
            User deletedUser = await _userDal.UpdateAsync(user);

            // Mapping the deleted user object to a DeletedUserDto object
            DeletedUserDto deletedUserDto = _mapper.Map<DeletedUserDto>(deletedUser);

            // Returning the deleted user DTO
            return deletedUserDto;
        }
    }
}
