using WebAPI.Repositories.Abstract;
using BusinessLayer.Features.OperationClaims.Constants;
using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;
using System.Security.Policy;

namespace BusinessLayer.Features.OperationClaims.Rules;

public class OperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IOperationClaimRepository _operationClaimDal;

    public OperationClaimBusinessRules(IOperationClaimRepository operationClaimDal)
    {
        _operationClaimDal = operationClaimDal;
    }

    public async Task OperationClaimIdShouldExistWhenSelected(int? id)
    {
        // Retrieves an operation claim from the data access layer (DAL) based on the provided ID
        OperationClaim? result = await _operationClaimDal.GetAsync(b => b.Id == id);

        // Throws an exception if the operation claim is not found
        if (result == null) throw new BusinessException(OperationClaimMessages.OperationClaimNotFound);
    }

    public async Task OperationClaimNameShouldBeNotExists(string name)
    {
        // Retrieves an operation claim from the data access layer (DAL) based on the provided name (case-insensitive)
        OperationClaim? user = await _operationClaimDal.GetAsync(u => u.Name.ToLower() == name.ToLower());

        // Throws an exception if an operation claim with the same name already exists
        if (user != null) throw new BusinessException(OperationClaimMessages.OperationClaimNameAlreadyExists);
    }

    public async Task OperationMustBeAvailable()
    {
        // Retrieves all operation claims from the data access layer (DAL)
        List<OperationClaim>? results = _operationClaimDal.GetAll();

        // Throws an exception if there are no operation claims available
        if (results.Count <= 0) throw new BusinessException(OperationClaimMessages.OperationClaimNotFound);
    }

}
