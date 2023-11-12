using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using MediatR;
using static BusinessLayer.Features.CustomerWarehouseCosts.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using BusinessLayer.Features.CustomerWarehouseCosts.Rules;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Commands.UpdateCustomerWarehouseCosts;

public class UpdateCustomerWarehouseCostCommand : IRequest<UpdatedCustomerWarehouseCostDto>/*, ISecuredRequest*/
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public double SetupCost { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerWarehouseCostUpdate };

    public class UpdateCustomerWarehouseCostCommandHandler : IRequestHandler<UpdateCustomerWarehouseCostCommand, UpdatedCustomerWarehouseCostDto>
    {
        private readonly ICustomerWarehouseCostRepository _customerWarehouseCostRepository;
        private readonly IMapper _mapper;
        private readonly CustomerWarehouseCostBusinessRules _customerWarehouseCostBusinessRules;

        public UpdateCustomerWarehouseCostCommandHandler(ICustomerWarehouseCostRepository customerWarehouseCostRepository, IMapper mapper, CustomerWarehouseCostBusinessRules customerWarehouseCostBusinessRules)
        {
            _customerWarehouseCostRepository = customerWarehouseCostRepository;
            _mapper = mapper;
            _customerWarehouseCostBusinessRules = customerWarehouseCostBusinessRules;
        }

        public async Task<UpdatedCustomerWarehouseCostDto> Handle(UpdateCustomerWarehouseCostCommand request, CancellationToken cancellationToken)
        {
            await _customerWarehouseCostBusinessRules.CustomerWarehouseCostIdShouldExistWhenSelected(request.Id);

            CustomerWarehouseCost mappedCustomerWarehouseCost = _mapper.Map<CustomerWarehouseCost>(request);
            CustomerWarehouseCost updateCustomerWarehouseCost = await _customerWarehouseCostRepository.UpdateAsync(mappedCustomerWarehouseCost);
            UpdatedCustomerWarehouseCostDto updatedCustomerWarehouseCostDto = _mapper.Map<UpdatedCustomerWarehouseCostDto>(updateCustomerWarehouseCost);

            return updatedCustomerWarehouseCostDto;
        }
    }
}
