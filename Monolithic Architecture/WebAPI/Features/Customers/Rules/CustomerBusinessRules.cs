using WebAPI.Repositories.Abstract;
using BusinessLayer.Features.OperationClaims.Constants;
using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Customers.Rules;

public class CustomerBusinessRules : BaseBusinessRules
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerBusinessRules(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task CustomerIdShouldExistWhenSelected(int? id)
    {
        Customer? result = await _customerRepository.GetAsync(b => b.Id == id);
        if (result == null) throw new BusinessException(OperationClaimMessages.OperationClaimNotFound);
    }

}
