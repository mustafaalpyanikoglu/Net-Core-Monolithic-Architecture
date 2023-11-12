using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using BusinessLayer.Features.CustomerWarehouseCosts.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.CustomerWarehouseCosts.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Commands.DeleteCustomerWarehouseCosts;

public class DeleteCustomerWarehouseCostCommand : IRequest<DeletedCustomerWarehouseCostDto>/*, ISecuredRequest*/
{
    public int Id { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerWarehouseCostDelete };

    public class DeleteCustomerWarehouseCostCommandHandler : IRequestHandler<DeleteCustomerWarehouseCostCommand, DeletedCustomerWarehouseCostDto>
    {
        private readonly ICustomerWarehouseCostRepository _customerWarehouseCostRepository;
        private readonly IMapper _mapper;
        private readonly CustomerWarehouseCostBusinessRules _customerWarehouseCostBusinessRules;

        public DeleteCustomerWarehouseCostCommandHandler(ICustomerWarehouseCostRepository customerWarehouseCostRepository, IMapper mapper, CustomerWarehouseCostBusinessRules customerWarehouseCostBusinessRules)
        {
            _customerWarehouseCostRepository = customerWarehouseCostRepository;
            _mapper = mapper;
            _customerWarehouseCostBusinessRules = customerWarehouseCostBusinessRules;
        }

        public async Task<DeletedCustomerWarehouseCostDto> Handle(DeleteCustomerWarehouseCostCommand request, CancellationToken cancellationToken)
        {
            await _customerWarehouseCostBusinessRules.CustomerWarehouseCostIdShouldExistWhenSelected(request.Id);

            CustomerWarehouseCost mappedCustomerWarehouseCost = _mapper.Map<CustomerWarehouseCost>(request);
            CustomerWarehouseCost deletedCustomerWarehouseCost = await _customerWarehouseCostRepository.DeleteAsync(mappedCustomerWarehouseCost);

            DeletedCustomerWarehouseCostDto deletedCustomerWarehouseCostDto = _mapper.Map<DeletedCustomerWarehouseCostDto>(deletedCustomerWarehouseCost);

            return deletedCustomerWarehouseCostDto;
        }
    }
}
