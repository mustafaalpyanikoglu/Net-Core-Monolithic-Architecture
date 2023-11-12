using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using BusinessLayer.Features.CustomerWarehouseCosts.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.CustomerWarehouseCosts.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Queries.GetByIdCustomerWarehouseCosts;

public class GetByIdCustomerWarehouseCostQuery : IRequest<CustomerWarehouseCostDto>//, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerWarehouseCostGet };

    public class GetByIdCustomerWarehouseCostQueryHandler : IRequestHandler<GetByIdCustomerWarehouseCostQuery, CustomerWarehouseCostDto>
    {
        private readonly ICustomerWarehouseCostRepository _customerWarehouseCostRepository;
        private readonly IMapper _mapper;
        private readonly CustomerWarehouseCostBusinessRules _customerWarehouseCostBusinessRules;

        public GetByIdCustomerWarehouseCostQueryHandler(ICustomerWarehouseCostRepository customerWarehouseCostRepository, IMapper mapper, CustomerWarehouseCostBusinessRules customerWarehouseCostBusinessRules)
        {
            _customerWarehouseCostRepository = customerWarehouseCostRepository;
            _mapper = mapper;
            _customerWarehouseCostBusinessRules = customerWarehouseCostBusinessRules;
        }

        public async Task<CustomerWarehouseCostDto> Handle(GetByIdCustomerWarehouseCostQuery request, CancellationToken cancellationToken)
        {
            await _customerWarehouseCostBusinessRules.CustomerWarehouseCostIdShouldExistWhenSelected(request.Id);

            CustomerWarehouseCost? customerWarehouseCost = await _customerWarehouseCostRepository.GetAsync(m => m.Id == request.Id);
            CustomerWarehouseCostDto customerWarehouseCostDto = _mapper.Map<CustomerWarehouseCostDto>(customerWarehouseCost);

            return customerWarehouseCostDto;

        }
    }
}
