using AutoMapper;
using MediatR;
using WebAPI.Application.Pipelines.Authorization;
using static BusinessLayer.Features.OperationClaims.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using BusinessLayer.Features.OperationClaims.Rules;
using WebAPI.Features.OperationClaims.Dtos;

namespace BusinessLayer.Features.OperationClaims.Commands.UpdateOperationClaim;

public class UpdateOperationClaimCommand : IRequest<UpdatedOperationClaimDto>, ISecuredRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string[] Roles => new[] { ADMIN, OperationClaimUpdate };

    public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdatedOperationClaimDto>
    {
        private readonly IOperationClaimRepository _operationClaimDal;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

        public UpdateOperationClaimCommandHandler(IOperationClaimRepository operationClaimDal, IMapper mapper, OperationClaimBusinessRules operationClaimBusinessRules)
        {
            _operationClaimDal = operationClaimDal;
            _mapper = mapper;
            _operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<UpdatedOperationClaimDto> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
        {
            await _operationClaimBusinessRules.OperationClaimIdShouldExistWhenSelected(request.Id);
            await _operationClaimBusinessRules.OperationClaimNameShouldBeNotExists(request.Name);

            // Mapping the request to an OperationClaim object
            OperationClaim mappedOperationClaim = _mapper.Map<OperationClaim>(request);

            // Updating the mapped operation claim in the operation claim data access layer (DAL)
            OperationClaim updatedOperationClaim = await _operationClaimDal.UpdateAsync(mappedOperationClaim);

            // Mapping the updated operation claim to an UpdatedOperationClaimDto object
            UpdatedOperationClaimDto updateOperationClaimDto = _mapper.Map<UpdatedOperationClaimDto>(updatedOperationClaim);

            return updateOperationClaimDto;
        }
    }
}
