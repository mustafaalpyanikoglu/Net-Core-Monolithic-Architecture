using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Customers.Dtos;
using BusinessLayer.Features.Customers.Rules;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Customers.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Customers.Queries.GetByIdCustomer;

public class GetByIdCustomerQuery : IRequest<CustomerDto>//, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerGet };

    public class GetByIdCustomerQueryHandler : IRequestHandler<GetByIdCustomerQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly CustomerBusinessRules _customerBusinessRules;

        public GetByIdCustomerQueryHandler(ICustomerRepository customerRepository, IMapper mapper, CustomerBusinessRules customerBusinessRules)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _customerBusinessRules = customerBusinessRules;
        }

        public async Task<CustomerDto> Handle(GetByIdCustomerQuery request, CancellationToken cancellationToken)
        {
            await _customerBusinessRules.CustomerIdShouldExistWhenSelected(request.Id);

            Customer? customer = await _customerRepository.GetAsync(m => m.Id == request.Id);
            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);

            return customerDto;

        }
    }
}
