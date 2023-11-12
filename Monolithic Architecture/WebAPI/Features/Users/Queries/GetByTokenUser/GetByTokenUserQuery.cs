using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BusinessLayer.Features.Users.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Users.Queries.GetByTokenUser;

public class GetByTokenUserQuery : IRequest<UserDto>, ISecuredRequest
{
    public string[] Roles => new[] { ADMIN, USER, INTERN };

    public class GetByTokenUserQueryHanlder : IRequestHandler<GetByTokenUserQuery, UserDto>
    {
        private readonly IUserRepository _userDal;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetByTokenUserQueryHanlder
            (IUserRepository userDal,
            IMapper mapper,
            UserBusinessRules userBusinessRules,
            IHttpContextAccessor httpContextAccessor)
        {
            _userDal = userDal;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> Handle(GetByTokenUserQuery request, CancellationToken cancellationToken)
        {
            List<Claim>? userClaims = _httpContextAccessor.HttpContext?.User.Claims.ToList();

            await _userBusinessRules.UserMustBeLoggedIn(userClaims);

            int userId = Convert.ToInt32(userClaims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            User? user = await _userDal.GetAsync
                 (
                     predicate: u => u.Id == userId,
                     include: u => u.Include(u => u.UserOperationClaims).ThenInclude(t => t.OperationClaim)
                 );

            await _userBusinessRules.UserShouldBeExist(user);

            UserDto userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
    }
}
