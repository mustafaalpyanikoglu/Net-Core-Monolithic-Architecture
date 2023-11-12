using WebAPI.Repositories.Abstract;
using AutoMapper;
using BusinessLayer.Features.Customers.Models;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Application.Requests;
using WebAPI.Persistence.Paging;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Customers.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Customers.Queries.GetListCustomer;

public class GetListCustomerQuery : IRequest<CustomerListModel>//, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public string[] Roles => new[] { ADMIN, CustomerGet };

    public class GetListCustomerQueryHanlder : IRequestHandler<GetListCustomerQuery, CustomerListModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetListCustomerQueryHanlder(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomerListModel> Handle(GetListCustomerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Customer> customers = await _customerRepository.GetListAsync(
                index: request.PageRequest.Page,
                size: request.PageRequest.PageSize);

            CustomerListModel mappedCustomerListModel = _mapper.Map<CustomerListModel>(customers);
            return mappedCustomerListModel;
        }
    }
}
