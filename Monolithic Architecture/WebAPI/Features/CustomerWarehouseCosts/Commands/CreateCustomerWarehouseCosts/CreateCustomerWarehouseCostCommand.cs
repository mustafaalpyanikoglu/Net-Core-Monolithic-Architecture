using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using BusinessLayer.Features.CustomerWarehouseCosts.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.CustomerWarehouseCosts.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Commands.CreateCustomerWarehouseCosts;

public class CreateCustomerWarehouseCostCommand : IRequest<CreatedCustomerWarehouseCostDto>/*, ISecuredRequest*/
{
    public int Capacity { get; set; }
    public double SetupCost { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerWarehouseCostAdd };

    public class CreateCustomerWarehouseCostCommandHandler : IRequestHandler<CreateCustomerWarehouseCostCommand, CreatedCustomerWarehouseCostDto>
    {
        private readonly ICustomerWarehouseCostRepository _customerWarehouseCostRepository;
        private readonly IMapper _mapper;
        private readonly CustomerWarehouseCostBusinessRules _customerWarehouseCostBusinessRules;

        public CreateCustomerWarehouseCostCommandHandler(ICustomerWarehouseCostRepository customerWarehouseCostRepository, IMapper mapper, CustomerWarehouseCostBusinessRules customerWarehouseCostBusinessRules)
        {
            _customerWarehouseCostRepository = customerWarehouseCostRepository;
            _mapper = mapper;
            _customerWarehouseCostBusinessRules = customerWarehouseCostBusinessRules;
        }

        public async Task<CreatedCustomerWarehouseCostDto> Handle(CreateCustomerWarehouseCostCommand request, CancellationToken cancellationToken)
        {
            CustomerWarehouseCost mappedCustomerWarehouseCost = _mapper.Map<CustomerWarehouseCost>(request);
            CustomerWarehouseCost createdCustomerWarehouseCost = await _customerWarehouseCostRepository.AddAsync(mappedCustomerWarehouseCost);

            CreatedCustomerWarehouseCostDto createdCustomerWarehouseCostDto = _mapper.Map<CreatedCustomerWarehouseCostDto>(createdCustomerWarehouseCost);

            return createdCustomerWarehouseCostDto;
        }
    }
}
