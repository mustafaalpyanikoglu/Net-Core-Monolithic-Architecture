using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.OperationClaims.Rules;
using BusinessLayer.Features.UserOperationClaims.Dtos;
using BusinessLayer.Features.UserOperationClaims.Rules;
using BusinessLayer.Features.Users.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.UserOperationClaims.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.UserOperationClaims.Commands.UpdateUserOperationClaim;

public class UpdateUserOperationClaimCommand : IRequest<UpdateUserOperationClaimDto>//,ISecuredRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OperationClaimId { get; set; }

    public string[] Roles => new[] { ADMIN, UserOperationClaimUpdate };

    public class UpdateUserOperationClaimCommandHandler : IRequestHandler<UpdateUserOperationClaimCommand, UpdateUserOperationClaimDto>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimDal;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

        public UpdateUserOperationClaimCommandHandler(IUserOperationClaimRepository userOperationClaimDal, IMapper mapper, UserOperationClaimBusinessRules userOperationClaimBusinessRules, UserBusinessRules userBusinessRules, OperationClaimBusinessRules operationClaimBusinessRules)
        {
            _userOperationClaimDal = userOperationClaimDal;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
            _userBusinessRules = userBusinessRules;
            _operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<UpdateUserOperationClaimDto> Handle(UpdateUserOperationClaimCommand request, CancellationToken cancellationToken)
        {
            await _userOperationClaimBusinessRules.UserOperationClaimIdMustBeAvailable(request.Id);
            await _userBusinessRules.UserIdMustBeAvailable(request.UserId);
            await _operationClaimBusinessRules.OperationClaimIdShouldExistWhenSelected(request.OperationClaimId);

            // Mapping the request to a UserOperationClaim object
            UserOperationClaim mappedUserOperationClaim = _mapper.Map<UserOperationClaim>(request);

            // Updating the mapped user operation claim in the user operation claim data access layer (DAL)
            UserOperationClaim updatedUserOperationClaim = await _userOperationClaimDal.UpdateAsync(mappedUserOperationClaim);

            // Mapping the updated user operation claim to an UpdateUserOperationClaimDto object
            UpdateUserOperationClaimDto updateUserOperationClaimDto = _mapper.Map<UpdateUserOperationClaimDto>(updatedUserOperationClaim);

            return updateUserOperationClaimDto;

        }
    }
}
