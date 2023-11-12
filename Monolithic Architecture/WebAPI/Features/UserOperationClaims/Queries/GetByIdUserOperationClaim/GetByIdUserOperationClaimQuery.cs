using AutoMapper;
using BusinessLayer.Features.UserOperationClaims.Dtos;
using BusinessLayer.Features.UserOperationClaims.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static BusinessLayer.Features.UserOperationClaims.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.UserOperationClaims.Queries.GetByIdUserOperationClaim;

public class GetByIdUserOperationClaimQuery : IRequest<UserOperationClaimDto>//, ISecuredRequest
{
    public int Id { get; set; }
    public string[] Roles => new[] { ADMIN, UserOperationClaimGet };

    public class GetByIdUserOperationClaimQueryHanlder : IRequestHandler<GetByIdUserOperationClaimQuery, UserOperationClaimDto>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimDal;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public GetByIdUserOperationClaimQueryHanlder(IUserOperationClaimRepository userOperationClaimDal, IMapper mapper, UserOperationClaimBusinessRules userOperationClaimBusinessRules)
        {
            _userOperationClaimDal = userOperationClaimDal;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<UserOperationClaimDto> Handle(GetByIdUserOperationClaimQuery request, CancellationToken cancellationToken)
        {
            await _userOperationClaimBusinessRules.UserOperationClaimIdMustBeAvailable(request.Id);

            // Retrieves a user operation claim from the data access layer (DAL) based on the provided ID, including related entities
            UserOperationClaim? userOperationClaim = await _userOperationClaimDal.GetAsync(
                u => u.Id == request.Id,
                include: c => c.Include(c => c.User).Include(c => c.OperationClaim)
            );

            // Maps the user operation claim to a UserOperationClaimDto object
            UserOperationClaimDto userOperationClaimDto = _mapper.Map<UserOperationClaimDto>(userOperationClaim);

            return userOperationClaimDto;

        }
    }
}
