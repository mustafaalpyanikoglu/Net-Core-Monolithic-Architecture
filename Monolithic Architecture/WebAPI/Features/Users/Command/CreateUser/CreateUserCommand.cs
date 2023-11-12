using AutoMapper;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Users.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Repositories.Abstract;
using WebAPI.Security.Constants;

namespace BusinessLayer.Features.Users.Command.CreateUser;

public class CreateUserCommand : IRequest<CreatedUserDto>/*, ISecuredRequest*/
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? ImageUrl { get; set; }
    public string? CVUrl { get; set; }

    //public string[] Roles => new[] { Admin, UserAdd };

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public CreateUserCommandHandler(IUserRepository userDal, IMapper mapper, UserBusinessRules userBusinessRules)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<CreatedUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserEmailMustNotExist(request.Email);

            // Mapping the properties from the request object to a User object
            User mappedUser = _mapper.Map<User>(request);

            // Creating a password hash and salt for the user's password
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);
            mappedUser.PasswordHash = passwordHash;
            mappedUser.PasswordSalt = passwordSalt;

            // Setting the registration date and intern status for the user
            mappedUser.RegistrationDate = DateTime.UtcNow;

            // Adding the mapped user to the user data access layer (DAL)
            User createdUser = await _userDal.AddAsync(mappedUser);

            // Mapping the created user object to a CreatedUserDto object
            CreatedUserDto createdUserDto = _mapper.Map<CreatedUserDto>(createdUser);

            // Returning the created user DTO
            return createdUserDto;

        }
    }
}
