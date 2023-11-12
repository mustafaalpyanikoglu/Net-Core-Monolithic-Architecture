using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.OperationClaims.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.OperationClaims.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Features.OperationClaims.Dtos;

namespace BusinessLayer.Features.OperationClaims.Queries.GetByIdOperationClaim;

public class GetByIdOperationClaimQuery : IRequest<OperationClaimDto>//, ISecuredRequest
{
    public int Id { get; set; }
    //public string[] Roles => new[] { Admin, OperationClaimGet };

    public class GetByIdOperationClaimQueryHandler : IRequestHandler<GetByIdOperationClaimQuery, OperationClaimDto>
    {
        private readonly IOperationClaimRepository _operationClaimDal;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

        public GetByIdOperationClaimQueryHandler(IOperationClaimRepository operationClaimDal, IMapper mapper, OperationClaimBusinessRules operationClaimBusinessRules)
        {
            _operationClaimDal = operationClaimDal;
            _mapper = mapper;
            _operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<OperationClaimDto> Handle(GetByIdOperationClaimQuery request, CancellationToken cancellationToken)
        {
            await _operationClaimBusinessRules.OperationClaimIdShouldExistWhenSelected(request.Id);

            // Retrieves an operation claim from the data access layer (DAL) based on the provided ID
            OperationClaim? operationClaim = await _operationClaimDal.GetAsync(m => m.Id == request.Id);

            // Maps the operation claim to an OperationClaimDto object
            OperationClaimDto operationClaimDto = _mapper.Map<OperationClaimDto>(operationClaim);

            return operationClaimDto;

        }
    }
}
