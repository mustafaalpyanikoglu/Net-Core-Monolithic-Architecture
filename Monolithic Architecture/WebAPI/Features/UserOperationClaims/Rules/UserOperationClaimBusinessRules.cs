using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;
using WebAPI.Repositories.Abstract;
using static BusinessLayer.Features.UserOperationClaims.Constants.UserOperationClaimMessages;

namespace BusinessLayer.Features.UserOperationClaims.Rules;

public class UserOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserOperationClaimRepository _userOperationClaimDal;

    public UserOperationClaimBusinessRules(IUserOperationClaimRepository userOperationClaimDal)
    {
        _userOperationClaimDal = userOperationClaimDal;
    }

    public async Task UserOperationClaimMustBeAvailable()
    {
        List<UserOperationClaim>? results = _userOperationClaimDal.GetAll();
        if (results.Count <= 0) throw new BusinessException(UserOperationClaimNotFound);
    }

    public async Task UserOperationClaimIdMustBeAvailable(int userOperationClaimId)
    {
        UserOperationClaim? result = await _userOperationClaimDal.GetAsync(t => t.Id == userOperationClaimId);
        if (result == null) throw new BusinessException(UserOperationClaimNotFound);
    }

    public async Task TheUserAlreadyHasSuchARole(int userId, int operationClaimId)
    {
        UserOperationClaim? result = await _userOperationClaimDal.GetAsync(b => b.UserId == userId && b.OperationClaimId == operationClaimId);
        if (result is not null) throw new BusinessException(TheUserAlreadyHasSuchRole);
    }
    public async Task TheUserAlreadyHasARole(int userId)
    {
        UserOperationClaim? result = await _userOperationClaimDal.GetAsync(p => p.UserId == userId);
        if (result is not null) throw new BusinessException(TheUserAlreadyHasRole);
    }
}
