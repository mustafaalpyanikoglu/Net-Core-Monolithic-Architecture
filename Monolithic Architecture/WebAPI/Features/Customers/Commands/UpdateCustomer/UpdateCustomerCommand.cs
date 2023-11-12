using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Customers.Dtos;
using BusinessLayer.Features.Customers.Rules;
using WebAPI.Application.Pipelines.Authorization;
using MediatR;
using static BusinessLayer.Features.Customers.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<UpdatedCustomerDto>/*, ISecuredRequest*/
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Demand { get; set; }

    public string[] Roles => new[] { ADMIN, CustomerUpdate };

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdatedCustomerDto>
    {
        private readonly IMapper _mapper;
        private readonly CustomerBusinessRules _customerBusinessRules;
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(IMapper mapper, CustomerBusinessRules customerBusinessRules, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerBusinessRules = customerBusinessRules;
            _customerRepository = customerRepository;
        }

        public async Task<UpdatedCustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _customerBusinessRules.CustomerIdShouldExistWhenSelected(request.Id);

            Customer mappedCustomer = _mapper.Map<Customer>(request);
            Customer updateCustomer = await _customerRepository.UpdateAsync(mappedCustomer);

            UpdatedCustomerDto updatedCustomerDto = _mapper.Map<UpdatedCustomerDto>(updateCustomer);

            return updatedCustomerDto;
        }
    }
}
