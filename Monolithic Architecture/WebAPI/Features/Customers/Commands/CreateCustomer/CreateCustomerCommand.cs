using AutoMapper;
using BusinessLayer.Features.Customers.Dtos;
using BusinessLayer.Features.Customers.Rules;
using MediatR;
using WebAPI.Models.Concrete;
using WebAPI.Repositories.Abstract;
using static BusinessLayer.Features.Customers.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<CreatedCustomerDto>/*, ISecuredRequest*/
{
    public int UserID { get; set; }
    public int Demand { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerAdd };

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreatedCustomerDto>
    {
        private readonly IMapper _mapper;
        private readonly CustomerBusinessRules _customerBusinessRules;
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(IMapper mapper, CustomerBusinessRules customerBusinessRules, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerBusinessRules = customerBusinessRules;
            _customerRepository = customerRepository;
        }

        public async Task<CreatedCustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer mappedCustomer = _mapper.Map<Customer>(request);
            Customer createdCustomer = await _customerRepository.AddAsync(mappedCustomer);

            CreatedCustomerDto createdCustomerDto = _mapper.Map<CreatedCustomerDto>(createdCustomer);

            return createdCustomerDto;
        }
    }
}
